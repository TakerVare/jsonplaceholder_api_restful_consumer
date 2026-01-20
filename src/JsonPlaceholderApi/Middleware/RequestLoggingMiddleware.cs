using System.Diagnostics;

namespace JsonPlaceholderApi.Middleware;

/// <summary>
/// Middleware para registrar informacion de cada peticion HTTP entrante.
///
/// POR QUE IMPLEMENTAR LOGGING DE REQUESTS:
/// 1. Observabilidad: Permite monitorear el trafico de la API en tiempo real,
///    identificando patrones de uso, endpoints mas utilizados, etc.
/// 2. Debugging: Facilita la identificacion de problemas al poder ver el flujo
///    completo de una peticion (metodo, ruta, tiempo de respuesta, codigo de estado).
/// 3. Auditoria: Mantiene un registro de todas las operaciones realizadas,
///    util para cumplimiento normativo y analisis de seguridad.
/// 4. Performance: Permite identificar endpoints lentos midiendo el tiempo de respuesta.
/// 5. Alertas: Los logs pueden alimentar sistemas de alertas para detectar anomalias
///    (picos de errores 500, tiempos de respuesta elevados, etc.).
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Intercepta cada request y registra informacion relevante.
    /// Usa Stopwatch para medir con precision el tiempo de ejecucion.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // POR QUE USAR STOPWATCH:
        // Stopwatch proporciona medicion de tiempo de alta precision,
        // mas preciso que DateTime.Now para medir duraciones cortas.
        var stopwatch = Stopwatch.StartNew();

        // Log de inicio de request
        // POR QUE LOGGEAR AL INICIO:
        // Permite ver requests que nunca terminaron (por timeout o crash),
        // ya que el log de finalizacion nunca se escribiria en esos casos.
        _logger.LogInformation(
            "Request iniciado: {Method} {Path} | TraceId: {TraceId}",
            context.Request.Method,
            context.Request.Path,
            context.TraceIdentifier);

        try
        {
            // Ejecuta el resto del pipeline
            await _next(context);
        }
        finally
        {
            // POR QUE USAR FINALLY:
            // Garantiza que el log de finalizacion se escriba siempre,
            // incluso si ocurre una excepcion (que sera manejada por
            // GlobalExceptionHandlerMiddleware).
            stopwatch.Stop();

            // Determina el nivel de log segun el codigo de estado
            // POR QUE VARIAR EL NIVEL DE LOG:
            // - Los errores (4xx, 5xx) deben destacar en los logs para facil identificacion
            // - Los requests exitosos (2xx) son informativos
            // - Esto facilita filtrar logs por severidad en herramientas de monitoreo
            var statusCode = context.Response.StatusCode;
            var logLevel = statusCode >= 500 ? LogLevel.Error
                         : statusCode >= 400 ? LogLevel.Warning
                         : LogLevel.Information;

            _logger.Log(
                logLevel,
                "Request completado: {Method} {Path} | Status: {StatusCode} | Duracion: {Duration}ms | TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                statusCode,
                stopwatch.ElapsedMilliseconds,
                context.TraceIdentifier);
        }
    }
}

/// <summary>
/// Extension para registrar el middleware de logging de forma limpia.
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
