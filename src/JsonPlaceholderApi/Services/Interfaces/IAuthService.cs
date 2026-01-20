using JsonPlaceholderApi.Models.Auth;

namespace JsonPlaceholderApi.Services.Interfaces;

/// <summary>
/// Interfaz para el servicio de autenticacion.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Autentica un usuario y genera un token JWT.
    /// </summary>
    Task<AuthResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Obtiene los permisos efectivos de un usuario (roles + directos).
    /// </summary>
    Task<List<string>> GetUserPermissionsAsync(int userId);

    /// <summary>
    /// Verifica si un usuario tiene un permiso especifico.
    /// </summary>
    Task<bool> HasPermissionAsync(int userId, string resource, string operation);

    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    Task<AppUser?> GetUserByIdAsync(int userId);

    /// <summary>
    /// Obtiene un usuario por su nombre de usuario.
    /// </summary>
    Task<AppUser?> GetUserByUsernameAsync(string username);
}
