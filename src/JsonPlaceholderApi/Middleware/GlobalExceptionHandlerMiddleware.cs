using System.Net;
using System.Text.Json;

namespace JsonPlaceholderApi.Middleware;

/// <summary>
/// Middleware para el manejo centralizado de excepciones en toda la aplicacion.
///
/// POR QUE USAR UN MIDDLEWARE DE ERRORES GLOBAL:
/// 1. Centralizacion: En lugar de manejar excepciones en cada controlador individualmente,
///    este middleware captura TODAS las excepciones no manejadas en un solo lugar.
/// 2. Consistencia: Garantiza que todas las respuestas de error tengan el mismo formato,
///    facilitando el consumo de la API por parte de los clientes.
/// 3. Seguridad: Evita exponer detalles internos de la aplicacion (stack traces, mensajes
///    de sistema) en produccion, mostrando solo mensajes genericos al usuario.
/// 4. Logging: Permite registrar todos los errores de forma centralizada para diagnostico.
/// 5. Separacion de responsabilidades: Los controladores se enfocan en la logica de negocio,
///    no en el manejo de errores.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Metodo principal que intercepta cada request HTTP.
    /// Envuelve la ejecucion del pipeline en un try-catch para capturar cualquier excepcion.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Continua con el siguiente middleware en el pipeline.
            // Si ocurre una excepcion en cualquier punto posterior, sera capturada aqui.
            await _next(context);
        }
        catch (Exception ex)
        {
            // Registra el error con todos los detalles para diagnostico
            _logger.LogError(ex,
                "Error no manejado en {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            // Genera una respuesta de error estandarizada
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Genera una respuesta HTTP de error estandarizada.
    ///
    /// POR QUE USAR ErrorResponse PERSONALIZADO:
    /// - Proporciona un formato consistente para todos los errores
    /// - Incluye TraceId para correlacionar errores con los logs
    /// - Permite diferenciar entre tipos de errores (400, 404, 500, etc.)
    /// </summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            TraceId = context.TraceIdentifier
        };

        // Determina el codigo de estado y mensaje segun el tipo de excepcion
        // POR QUE CLASIFICAR EXCEPCIONES:
        // Diferentes tipos de errores requieren diferentes codigos HTTP para
        // comunicar correctamente al cliente que tipo de problema ocurrio
        switch (exception)
        {
            case HttpRequestException httpEx:
                // Error al comunicarse con JSONPlaceholder API
                context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                response.StatusCode = (int)HttpStatusCode.BadGateway;
                response.Message = "Error al comunicarse con el servicio externo";
                response.Detail = "La API de JSONPlaceholder no esta disponible o no respondio correctamente";
                break;

            case TaskCanceledException:
                // Timeout en la peticion
                context.Response.StatusCode = (int)HttpStatusCode.GatewayTimeout;
                response.StatusCode = (int)HttpStatusCode.GatewayTimeout;
                response.Message = "Tiempo de espera agotado";
                response.Detail = "La peticion al servicio externo excedio el tiempo limite";
                break;

            case ArgumentException argEx:
                // Error de validacion de argumentos
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Solicitud invalida";
                response.Detail = argEx.Message;
                break;

            case KeyNotFoundException:
                // Recurso no encontrado
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = "Recurso no encontrado";
                response.Detail = "El recurso solicitado no existe";
                break;

            default:
                // Error interno no esperado
                // POR QUE NO EXPONER DETALLES EN PRODUCCION:
                // Los mensajes de error internos pueden contener informacion sensible
                // (rutas de archivos, nombres de tablas, etc.) que podrian ser
                // aprovechados por atacantes
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "Error interno del servidor";
                response.Detail = "Ha ocurrido un error inesperado. Por favor, contacte al administrador.";
                break;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}

/// <summary>
/// Modelo estandarizado para respuestas de error.
///
/// POR QUE USAR UN MODELO DE ERROR ESTANDARIZADO:
/// 1. Los clientes de la API pueden parsear errores de forma predecible
/// 2. Facilita la documentacion de la API (Swagger puede mostrar este esquema)
/// 3. Incluye informacion util para debugging sin exponer detalles sensibles
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Codigo de estado HTTP del error
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Mensaje general del error (seguro para mostrar al usuario)
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detalle adicional del error
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// Identificador unico de la peticion para correlacionar con logs.
    /// POR QUE INCLUIR TRACEID:
    /// Permite a los desarrolladores buscar en los logs el error exacto
    /// que experimento un usuario especifico, facilitando el diagnostico.
    /// </summary>
    public string TraceId { get; set; } = string.Empty;
}

/// <summary>
/// Extension para registrar el middleware de forma limpia en Program.cs.
///
/// POR QUE USAR EXTENSION METHODS:
/// - Mantiene Program.cs limpio y legible
/// - Sigue el patron de configuracion de ASP.NET Core
/// - Facilita el descubrimiento del middleware via IntelliSense
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
