namespace JsonPlaceholderApi.Models.Auth;

/// <summary>
/// Representa un permiso granular en el sistema.
///
/// POR QUE PERMISOS GRANULARES:
/// 1. Control fino sobre quien puede hacer que en cada recurso
/// 2. Flexibilidad para crear roles personalizados
/// 3. Facilita auditorias de seguridad
/// 4. Principio de minimo privilegio: usuarios solo tienen lo necesario
///
/// Formato: {Recurso}.{Operacion}
/// Ejemplos: Posts.Create, Posts.Delete, Users.Read, Albums.Update
/// </summary>
public class Permission
{
    /// <summary>
    /// Recurso sobre el que aplica el permiso.
    /// Valores: Posts, Comments, Users, Albums, Photos, Todos
    /// </summary>
    public string Resource { get; set; } = string.Empty;

    /// <summary>
    /// Operacion permitida sobre el recurso.
    /// Valores: Create, Read, Update, Delete
    /// </summary>
    public string Operation { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del permiso (Resource.Operation)
    /// </summary>
    public string FullName => $"{Resource}.{Operation}";

    public Permission() { }

    public Permission(string resource, string operation)
    {
        Resource = resource;
        Operation = operation;
    }

    /// <summary>
    /// Crea un permiso desde su nombre completo (ej: "Posts.Create")
    /// </summary>
    public static Permission FromFullName(string fullName)
    {
        var parts = fullName.Split('.');
        if (parts.Length != 2)
            throw new ArgumentException($"Formato de permiso invalido: {fullName}. Esperado: Resource.Operation");

        return new Permission(parts[0], parts[1]);
    }
}

/// <summary>
/// Constantes para recursos disponibles.
/// POR QUE USAR CONSTANTES:
/// - Evita errores de tipeo
/// - Autocompletado en IDE
/// - Facilita refactoring
/// </summary>
public static class Resources
{
    public const string Posts = "Posts";
    public const string Comments = "Comments";
    public const string Users = "Users";
    public const string Albums = "Albums";
    public const string Photos = "Photos";
    public const string Todos = "Todos";
}

/// <summary>
/// Constantes para operaciones disponibles (CRUD).
/// </summary>
public static class Operations
{
    public const string Create = "Create";
    public const string Read = "Read";
    public const string Update = "Update";
    public const string Delete = "Delete";
}
