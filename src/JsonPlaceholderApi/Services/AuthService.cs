using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JsonPlaceholderApi.Models.Auth;
using JsonPlaceholderApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JsonPlaceholderApi.Services;

/// <summary>
/// Servicio de autenticacion que maneja login, registro y generacion de tokens JWT.
///
/// POR QUE CENTRALIZAR LA AUTENTICACION EN UN SERVICIO:
/// 1. Reutilizable desde diferentes controladores
/// 2. Facilita testing mediante mocks
/// 3. Separa la logica de autenticacion de los controladores
/// 4. Unico punto de cambio si se modifica la estrategia de auth
/// </summary>
public class AuthService : IAuthService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    // Almacen en memoria de usuarios (en produccion seria una base de datos)
    // POR QUE EN MEMORIA PARA ESTE PROYECTO:
    // - JSONPlaceholder no tiene sistema de usuarios real
    // - Simplifica la implementacion sin necesidad de base de datos
    // - Suficiente para demostrar el concepto de JWT
    private static readonly List<AppUser> _users = [];
    private static int _nextUserId = 1;
    private static readonly object _lock = new();

    public AuthService(IOptions<JwtSettings> jwtSettings, ILogger<AuthService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;

        // Crear usuario admin por defecto si no existe
        InitializeDefaultUsers();
    }

    /// <summary>
    /// Inicializa usuarios por defecto para pruebas.
    /// POR QUE USUARIOS POR DEFECTO:
    /// Facilita las pruebas sin necesidad de registrar usuarios primero.
    /// </summary>
    private void InitializeDefaultUsers()
    {
        lock (_lock)
        {
            if (_users.Count == 0)
            {
                // Admin por defecto
                _users.Add(new AppUser
                {
                    Id = _nextUserId++,
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = HashPassword("admin123"),
                    Roles = ["Admin"],
                    IsActive = true
                });

                // Editor por defecto
                _users.Add(new AppUser
                {
                    Id = _nextUserId++,
                    Username = "editor",
                    Email = "editor@example.com",
                    PasswordHash = HashPassword("editor123"),
                    Roles = ["Editor"],
                    IsActive = true
                });

                // Viewer por defecto
                _users.Add(new AppUser
                {
                    Id = _nextUserId++,
                    Username = "viewer",
                    Email = "viewer@example.com",
                    PasswordHash = HashPassword("viewer123"),
                    Roles = ["Viewer"],
                    IsActive = true
                });

                _logger.LogInformation("Usuarios por defecto inicializados: admin, editor, viewer");
            }
        }
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Intento de login para usuario: {Username}", request.Username);

        var user = await GetUserByUsernameAsync(request.Username);

        if (user == null)
        {
            _logger.LogWarning("Login fallido: usuario {Username} no encontrado", request.Username);
            return new AuthResponse
            {
                Success = false,
                Message = "Credenciales invalidas"
            };
        }

        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login fallido: contrasena incorrecta para {Username}", request.Username);
            return new AuthResponse
            {
                Success = false,
                Message = "Credenciales invalidas"
            };
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login fallido: usuario {Username} inactivo", request.Username);
            return new AuthResponse
            {
                Success = false,
                Message = "Usuario inactivo"
            };
        }

        var token = GenerateJwtToken(user);
        var permissions = await GetUserPermissionsAsync(user.Id);

        _logger.LogInformation("Login exitoso para usuario: {Username}", request.Username);

        return new AuthResponse
        {
            Success = true,
            Message = "Login exitoso",
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            User = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = user.Roles,
                Permissions = permissions
            }
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        _logger.LogInformation("Intento de registro para usuario: {Username}", request.Username);

        // Verificar si el usuario ya existe
        var existingUser = await GetUserByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            _logger.LogWarning("Registro fallido: usuario {Username} ya existe", request.Username);
            return new AuthResponse
            {
                Success = false,
                Message = "El nombre de usuario ya esta en uso"
            };
        }

        // Verificar si el email ya existe
        lock (_lock)
        {
            if (_users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Registro fallido: email {Email} ya existe", request.Email);
                return new AuthResponse
                {
                    Success = false,
                    Message = "El email ya esta registrado"
                };
            }
        }

        // Crear nuevo usuario
        var newUser = new AppUser
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Roles = request.Roles?.Count > 0 ? request.Roles : ["Viewer"], // Rol por defecto
            IsActive = true
        };

        lock (_lock)
        {
            newUser.Id = _nextUserId++;
            _users.Add(newUser);
        }

        var token = GenerateJwtToken(newUser);
        var permissions = await GetUserPermissionsAsync(newUser.Id);

        _logger.LogInformation("Registro exitoso para usuario: {Username} con roles: {Roles}",
            request.Username, string.Join(", ", newUser.Roles));

        return new AuthResponse
        {
            Success = true,
            Message = "Registro exitoso",
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            User = new UserInfo
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Email = newUser.Email,
                Roles = newUser.Roles,
                Permissions = permissions
            }
        };
    }

    /// <summary>
    /// Genera un token JWT para el usuario.
    ///
    /// POR QUE INCLUIR ESTOS CLAIMS:
    /// - sub (Subject): ID unico del usuario
    /// - name: Nombre de usuario para mostrar
    /// - email: Email del usuario
    /// - role: Roles del usuario (puede ser multiple)
    /// - permissions: Permisos granulares (custom claim)
    /// </summary>
    private string GenerateJwtToken(AppUser user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID unico del token
        };

        // Agregar roles como claims
        // POR QUE CLAIMS SEPARADOS POR ROL:
        // Permite usar [Authorize(Roles = "Admin")] en los controladores
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Agregar permisos como claims
        var permissions = GetPermissionsForRoles(user.Roles);
        foreach (var permission in user.DirectPermissions)
        {
            if (!permissions.Contains(permission.FullName))
                permissions.Add(permission.FullName);
        }

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task<List<string>> GetUserPermissionsAsync(int userId)
    {
        AppUser? user;
        lock (_lock)
        {
            user = _users.FirstOrDefault(u => u.Id == userId);
        }

        if (user == null)
            return Task.FromResult(new List<string>());

        var permissions = GetPermissionsForRoles(user.Roles);

        // Agregar permisos directos
        foreach (var permission in user.DirectPermissions)
        {
            if (!permissions.Contains(permission.FullName))
                permissions.Add(permission.FullName);
        }

        return Task.FromResult(permissions);
    }

    /// <summary>
    /// Obtiene los permisos combinados de una lista de roles.
    /// </summary>
    private List<string> GetPermissionsForRoles(List<string> roleNames)
    {
        var permissions = new HashSet<string>();

        foreach (var roleName in roleNames)
        {
            var role = PredefinedRoles.GetByName(roleName);
            if (role != null)
            {
                foreach (var permission in role.Permissions)
                {
                    permissions.Add(permission.FullName);
                }
            }
        }

        return permissions.ToList();
    }

    public Task<bool> HasPermissionAsync(int userId, string resource, string operation)
    {
        var permissionName = $"{resource}.{operation}";
        var permissions = GetUserPermissionsAsync(userId).Result;
        return Task.FromResult(permissions.Contains(permissionName));
    }

    public Task<AppUser?> GetUserByIdAsync(int userId)
    {
        lock (_lock)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == userId));
        }
    }

    public Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        lock (_lock)
        {
            return Task.FromResult(_users.FirstOrDefault(
                u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));
        }
    }

    /// <summary>
    /// Genera hash de contrasena usando PBKDF2.
    ///
    /// POR QUE PBKDF2:
    /// - Algoritmo recomendado por NIST para hash de contrasenas
    /// - Incluye salt automaticamente
    /// - Configurable en iteraciones (mas iteraciones = mas seguro pero mas lento)
    /// </summary>
    private static string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        var hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Verifica una contrasena contra su hash.
    /// </summary>
    private static bool VerifyPassword(string password, string storedHash)
    {
        var hashBytes = Convert.FromBase64String(storedHash);

        var salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        for (int i = 0; i < 32; i++)
        {
            if (hashBytes[i + 16] != hash[i])
                return false;
        }

        return true;
    }
}
