using Microsoft.AspNetCore.Authorization;

namespace JsonPlaceholderApi.Auth;

/// <summary>
/// Requisito de autorizacion basado en permisos granulares.
///
/// POR QUE USAR POLICY-BASED AUTHORIZATION:
/// 1. Mas flexible que role-based authorization
/// 2. Permite combinaciones complejas de requisitos
/// 3. Reutilizable en multiples controladores
/// 4. Facilita testing (se puede mockear el handler)
/// 5. Sigue el patron de ASP.NET Core
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Recurso sobre el que se requiere permiso.
    /// </summary>
    public string Resource { get; }

    /// <summary>
    /// Operacion que se requiere sobre el recurso.
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Nombre completo del permiso (Resource.Operation).
    /// </summary>
    public string PermissionName => $"{Resource}.{Operation}";

    public PermissionRequirement(string resource, string operation)
    {
        Resource = resource;
        Operation = operation;
    }
}

/// <summary>
/// Handler que evalua si el usuario cumple con el requisito de permiso.
///
/// POR QUE UN HANDLER SEPARADO:
/// - Separation of concerns: el requisito define QUE se necesita,
///   el handler define COMO se evalua
/// - Permite diferentes estrategias de evaluacion
/// - Facilita testing y mantenimiento
/// </summary>
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ILogger<PermissionHandler> _logger;

    public PermissionHandler(ILogger<PermissionHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Obtener el claim de identidad del usuario
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Autorizacion fallida: no se encontro ID de usuario en los claims");
            return Task.CompletedTask;
        }

        // Buscar el permiso requerido en los claims del usuario
        // POR QUE BUSCAR EN CLAIMS:
        // Los permisos se agregaron al token JWT en el momento del login,
        // no necesitamos consultar la base de datos en cada request.
        var hasPermission = context.User.Claims
            .Where(c => c.Type == "permission")
            .Any(c => c.Value.Equals(requirement.PermissionName, StringComparison.OrdinalIgnoreCase));

        if (hasPermission)
        {
            _logger.LogDebug("Autorizacion exitosa: usuario {UserId} tiene permiso {Permission}",
                userId, requirement.PermissionName);
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("Autorizacion fallida: usuario {UserId} no tiene permiso {Permission}",
                userId, requirement.PermissionName);
        }

        return Task.CompletedTask;
    }
}
