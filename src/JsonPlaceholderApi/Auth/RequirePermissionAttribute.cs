using Microsoft.AspNetCore.Authorization;

namespace JsonPlaceholderApi.Auth;

/// <summary>
/// Atributo para requerir un permiso especifico en un endpoint.
///
/// POR QUE UN ATRIBUTO PERSONALIZADO:
/// 1. Sintaxis mas limpia: [RequirePermission("Posts", "Create")]
/// 2. Evita errores de tipeo en nombres de politicas
/// 3. Autocompletado en IDE
/// 4. Documentacion integrada
///
/// USO:
/// [RequirePermission(Resources.Posts, Operations.Create)]
/// public async Task<ActionResult> CreatePost(...) { }
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Crea un requisito de permiso con recurso y operacion.
    /// </summary>
    /// <param name="resource">Recurso (Posts, Comments, Users, etc.)</param>
    /// <param name="operation">Operacion (Create, Read, Update, Delete)</param>
    public RequirePermissionAttribute(string resource, string operation)
        : base(policy: $"{resource}.{operation}")
    {
    }
}

/// <summary>
/// Extension para registrar todas las politicas de permisos.
///
/// POR QUE EXTENSION METHOD:
/// - Mantiene Program.cs limpio
/// - Centraliza la configuracion de politicas
/// - Facilita agregar nuevas politicas
/// </summary>
public static class PermissionPolicyExtensions
{
    /// <summary>
    /// Registra todas las politicas de permisos basadas en recursos y operaciones.
    /// </summary>
    public static AuthorizationOptions AddPermissionPolicies(this AuthorizationOptions options)
    {
        // POR QUE GENERAR POLITICAS DINAMICAMENTE:
        // Evita duplicacion de codigo y errores de sincronizacion
        // entre permisos definidos y politicas registradas.

        var resources = new[] { "Posts", "Comments", "Users", "Albums", "Photos", "Todos" };
        var operations = new[] { "Create", "Read", "Update", "Delete" };

        foreach (var resource in resources)
        {
            foreach (var operation in operations)
            {
                var policyName = $"{resource}.{operation}";
                options.AddPolicy(policyName, policy =>
                    policy.Requirements.Add(new PermissionRequirement(resource, operation)));
            }
        }

        return options;
    }
}
