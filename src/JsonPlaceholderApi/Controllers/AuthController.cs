using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JsonPlaceholderApi.Models.Auth;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Controllers;

/// <summary>
/// Controlador para autenticacion y gestion de usuarios.
///
/// POR QUE UN CONTROLADOR SEPARADO PARA AUTH:
/// 1. Separacion clara de responsabilidades
/// 2. Los endpoints de auth tienen requisitos diferentes (no requieren token)
/// 3. Facilita aplicar politicas de seguridad especificas
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Autentica un usuario y devuelve un token JWT.
    /// </summary>
    /// <remarks>
    /// Usuarios de prueba disponibles:
    /// - admin / admin123 (rol Admin - acceso total)
    /// - editor / editor123 (rol Editor - puede crear y modificar)
    /// - viewer / viewer123 (rol Viewer - solo lectura)
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous] // POR QUE AllowAnonymous: El login debe ser accesible sin token
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Solicitud de login recibida para: {Username}", request.Username);

        var response = await _authService.LoginAsync(request);

        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    /// <remarks>
    /// Roles disponibles: Admin, Editor, Viewer.
    /// Si no se especifica rol, se asigna "Viewer" por defecto.
    /// </remarks>
    [HttpPost("register")]
    [AllowAnonymous] // POR QUE AllowAnonymous: Permitir registro sin autenticacion previa
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Solicitud de registro recibida para: {Username}", request.Username);

        var response = await _authService.RegisterAsync(request);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Obtiene la informacion del usuario autenticado actualmente.
    /// </summary>
    [HttpGet("me")]
    [Authorize] // POR QUE Authorize: Solo usuarios autenticados pueden ver su perfil
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserInfo>> GetCurrentUser()
    {
        // Obtener el ID del usuario desde los claims del token
        // POR QUE DESDE CLAIMS:
        // El token ya fue validado por el middleware de autenticacion,
        // podemos confiar en los claims que contiene.
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
            ?? User.FindFirst("sub");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized();
        }

        var permissions = await _authService.GetUserPermissionsAsync(userId);

        return Ok(new UserInfo
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Roles = user.Roles,
            Permissions = permissions
        });
    }

    /// <summary>
    /// Lista los roles disponibles en el sistema.
    /// </summary>
    [HttpGet("roles")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetAvailableRoles()
    {
        // POR QUE EXPONER ROLES:
        // Permite a los clientes saber que roles pueden asignar al registrar usuarios
        var roles = PredefinedRoles.All.Select(r => new
        {
            r.Name,
            r.Description,
            Permissions = r.Permissions.Select(p => p.FullName)
        });

        return Ok(roles);
    }

    /// <summary>
    /// Verifica si el token actual es valido.
    /// </summary>
    [HttpGet("validate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult ValidateToken()
    {
        // POR QUE ENDPOINT DE VALIDACION:
        // Permite a los clientes verificar si su token sigue siendo valido
        // sin hacer una operacion real.
        return Ok(new { valid = true, message = "Token valido" });
    }
}
