namespace JsonPlaceholderApi.Models.Auth;

/// <summary>
/// Representa un rol que agrupa multiples permisos.
///
/// POR QUE USAR ROLES:
/// 1. Simplifica la asignacion de permisos (asignar un rol vs muchos permisos)
/// 2. Facilita la gestion de usuarios con responsabilidades similares
/// 3. Patron estandar en sistemas de autorizacion (RBAC - Role Based Access Control)
/// </summary>
public class Role
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Permission> Permissions { get; set; } = [];

    public Role() { }

    public Role(string name, string description, List<Permission> permissions)
    {
        Name = name;
        Description = description;
        Permissions = permissions;
    }
}

/// <summary>
/// Roles predefinidos del sistema.
///
/// POR QUE ROLES PREDEFINIDOS:
/// - Proporciona una configuracion inicial funcional
/// - Documenta los niveles de acceso esperados
/// - Facilita la implementacion inicial sin configuracion adicional
/// </summary>
public static class PredefinedRoles
{
    /// <summary>
    /// Administrador: acceso total a todos los recursos y operaciones.
    /// </summary>
    public static Role Admin => new(
        "Admin",
        "Administrador con acceso total al sistema",
        [
            // Posts - todos los permisos
            new Permission(Resources.Posts, Operations.Create),
            new Permission(Resources.Posts, Operations.Read),
            new Permission(Resources.Posts, Operations.Update),
            new Permission(Resources.Posts, Operations.Delete),
            // Comments - todos los permisos
            new Permission(Resources.Comments, Operations.Create),
            new Permission(Resources.Comments, Operations.Read),
            new Permission(Resources.Comments, Operations.Update),
            new Permission(Resources.Comments, Operations.Delete),
            // Users - todos los permisos
            new Permission(Resources.Users, Operations.Create),
            new Permission(Resources.Users, Operations.Read),
            new Permission(Resources.Users, Operations.Update),
            new Permission(Resources.Users, Operations.Delete),
            // Albums - todos los permisos
            new Permission(Resources.Albums, Operations.Create),
            new Permission(Resources.Albums, Operations.Read),
            new Permission(Resources.Albums, Operations.Update),
            new Permission(Resources.Albums, Operations.Delete),
            // Photos - todos los permisos
            new Permission(Resources.Photos, Operations.Create),
            new Permission(Resources.Photos, Operations.Read),
            new Permission(Resources.Photos, Operations.Update),
            new Permission(Resources.Photos, Operations.Delete),
            // Todos - todos los permisos
            new Permission(Resources.Todos, Operations.Create),
            new Permission(Resources.Todos, Operations.Read),
            new Permission(Resources.Todos, Operations.Update),
            new Permission(Resources.Todos, Operations.Delete),
        ]
    );

    /// <summary>
    /// Editor: puede crear y modificar contenido, pero no eliminar.
    /// </summary>
    public static Role Editor => new(
        "Editor",
        "Puede crear y modificar contenido, sin poder eliminar",
        [
            // Posts - crear, leer, actualizar
            new Permission(Resources.Posts, Operations.Create),
            new Permission(Resources.Posts, Operations.Read),
            new Permission(Resources.Posts, Operations.Update),
            // Comments - crear, leer, actualizar
            new Permission(Resources.Comments, Operations.Create),
            new Permission(Resources.Comments, Operations.Read),
            new Permission(Resources.Comments, Operations.Update),
            // Albums - crear, leer, actualizar
            new Permission(Resources.Albums, Operations.Create),
            new Permission(Resources.Albums, Operations.Read),
            new Permission(Resources.Albums, Operations.Update),
            // Photos - crear, leer, actualizar
            new Permission(Resources.Photos, Operations.Create),
            new Permission(Resources.Photos, Operations.Read),
            new Permission(Resources.Photos, Operations.Update),
            // Todos - crear, leer, actualizar
            new Permission(Resources.Todos, Operations.Create),
            new Permission(Resources.Todos, Operations.Read),
            new Permission(Resources.Todos, Operations.Update),
            // Users - solo lectura
            new Permission(Resources.Users, Operations.Read),
        ]
    );

    /// <summary>
    /// Viewer: solo lectura en todos los recursos.
    /// </summary>
    public static Role Viewer => new(
        "Viewer",
        "Solo puede ver contenido, sin modificar",
        [
            new Permission(Resources.Posts, Operations.Read),
            new Permission(Resources.Comments, Operations.Read),
            new Permission(Resources.Users, Operations.Read),
            new Permission(Resources.Albums, Operations.Read),
            new Permission(Resources.Photos, Operations.Read),
            new Permission(Resources.Todos, Operations.Read),
        ]
    );

    /// <summary>
    /// Obtiene todos los roles predefinidos.
    /// </summary>
    public static List<Role> All => [Admin, Editor, Viewer];

    /// <summary>
    /// Busca un rol por nombre.
    /// </summary>
    public static Role? GetByName(string name) =>
        All.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}
