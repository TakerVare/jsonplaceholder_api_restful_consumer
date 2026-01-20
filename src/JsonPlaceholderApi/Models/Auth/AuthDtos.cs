using System.ComponentModel.DataAnnotations;

namespace JsonPlaceholderApi.Models.Auth;

/// <summary>
/// DTO para solicitud de login.
///
/// POR QUE USAR DTOs SEPARADOS:
/// 1. No exponer el modelo interno (AppUser) en la API
/// 2. Validacion especifica para cada operacion
/// 3. Evitar over-posting (enviar campos no deseados)
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contrasena es requerida")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO para solicitud de registro.
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 50 caracteres")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es valido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contrasena es requerida")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contrasena debe tener al menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Roles a asignar al usuario (opcional).
    /// Si no se especifica, se asigna rol "Viewer" por defecto.
    /// </summary>
    public List<string>? Roles { get; set; }
}

/// <summary>
/// DTO para respuesta de autenticacion exitosa.
/// </summary>
public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Token JWT para usar en el header Authorization.
    /// POR QUE DEVOLVER EL TOKEN:
    /// El cliente debe incluirlo en las peticiones posteriores:
    /// Authorization: Bearer {token}
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Fecha de expiracion del token.
    /// POR QUE INCLUIR EXPIRACION:
    /// Permite al cliente saber cuando renovar el token sin decodificarlo.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Informacion del usuario autenticado.
    /// </summary>
    public UserInfo? User { get; set; }
}

/// <summary>
/// Informacion publica del usuario (sin datos sensibles).
/// </summary>
public class UserInfo
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];

    /// <summary>
    /// Lista de permisos efectivos (de roles + directos).
    /// POR QUE INCLUIR PERMISOS EN LA RESPUESTA:
    /// Permite al frontend saber que acciones puede realizar el usuario
    /// sin hacer peticiones adicionales.
    /// </summary>
    public List<string> Permissions { get; set; } = [];
}
