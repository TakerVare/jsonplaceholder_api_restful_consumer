namespace JsonPlaceholderApi.Models.Auth;

/// <summary>
/// Representa un usuario del sistema de autenticacion.
///
/// POR QUE CREAR UN MODELO DE USUARIO SEPARADO:
/// 1. El modelo User de JSONPlaceholder representa datos externos
/// 2. Este modelo representa usuarios de NUESTRA API con credenciales
/// 3. Separacion clara entre datos externos y datos de autenticacion
/// </summary>
public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash de la contrasena (nunca almacenar en texto plano).
    /// POR QUE HASH Y NO TEXTO PLANO:
    /// - Si la base de datos es comprometida, las contrasenas no se exponen
    /// - Cumple con estandares de seguridad (OWASP)
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Roles asignados al usuario.
    /// Un usuario puede tener multiples roles (ej: Admin + Editor)
    /// </summary>
    public List<string> Roles { get; set; } = [];

    /// <summary>
    /// Permisos especificos del usuario (adicionales a los de sus roles).
    /// POR QUE PERMISOS DIRECTOS ADEMAS DE ROLES:
    /// Permite asignar permisos especificos sin crear un rol nuevo.
    /// Ej: Un usuario normal con permiso especial para eliminar posts.
    /// </summary>
    public List<Permission> DirectPermissions { get; set; } = [];

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
