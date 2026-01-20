using JsonPlaceholderApi.Middleware;
using JsonPlaceholderApi.Repositories;
using JsonPlaceholderApi.Repositories.Interfaces;
using JsonPlaceholderApi.Services;
using JsonPlaceholderApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CONFIGURACION DE SERVICIOS
// ============================================================================

// Add controllers
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "JSONPlaceholder API Consumer",
        Version = "v1",
        Description = "API RESTful que consume datos de JSONPlaceholder"
    });
});

// ----------------------------------------------------------------------------
// CONFIGURACION DE HTTPCLIENT
// ----------------------------------------------------------------------------
// POR QUE USAR IHttpClientFactory (AddHttpClient):
// 1. Evita el agotamiento de sockets (socket exhaustion) que ocurre al crear
//    multiples instancias de HttpClient manualmente.
// 2. Gestiona automaticamente el ciclo de vida de los HttpMessageHandlers,
//    renovandolos periodicamente para respetar cambios en DNS.
// 3. Permite configuracion centralizada (BaseAddress, Timeout, Headers).
// 4. Facilita el testing mediante la inyeccion de handlers mock.
// ----------------------------------------------------------------------------

// Obtener configuracion desde appsettings.json
var jsonPlaceholderBaseUrl = builder.Configuration["JsonPlaceholderApi:BaseUrl"]
    ?? "https://jsonplaceholder.typicode.com/";
var timeoutSeconds = builder.Configuration.GetValue<int>("JsonPlaceholderApi:TimeoutSeconds", 30);

builder.Services.AddHttpClient<IPostRepository, PostRepository>(client =>
{
    client.BaseAddress = new Uri(jsonPlaceholderBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});

builder.Services.AddHttpClient<ICommentRepository, CommentRepository>(client =>
{
    client.BaseAddress = new Uri(jsonPlaceholderBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});

builder.Services.AddHttpClient<IUserRepository, UserRepository>(client =>
{
    client.BaseAddress = new Uri(jsonPlaceholderBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});

builder.Services.AddHttpClient<IAlbumRepository, AlbumRepository>(client =>
{
    client.BaseAddress = new Uri(jsonPlaceholderBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});

builder.Services.AddHttpClient<IPhotoRepository, PhotoRepository>(client =>
{
    client.BaseAddress = new Uri(jsonPlaceholderBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});

builder.Services.AddHttpClient<ITodoRepository, TodoRepository>(client =>
{
    client.BaseAddress = new Uri(jsonPlaceholderBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});

// ----------------------------------------------------------------------------
// REGISTRO DE SERVICIOS
// ----------------------------------------------------------------------------
// POR QUE USAR AddScoped:
// - Scoped: Una instancia por request HTTP. Ideal para servicios que mantienen
//   estado durante una peticion pero no entre peticiones diferentes.
// - Otras opciones: Singleton (una instancia global), Transient (nueva instancia
//   cada vez que se solicita).
// ----------------------------------------------------------------------------
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

// ============================================================================
// CONFIGURACION DEL PIPELINE DE MIDDLEWARES
// ============================================================================
// POR QUE EL ORDEN DE LOS MIDDLEWARES ES IMPORTANTE:
// Los middlewares se ejecutan en el orden en que se registran (de arriba a abajo)
// para las peticiones entrantes, y en orden inverso para las respuestas.
//
// Orden recomendado:
// 1. Manejo de excepciones (primero, para capturar errores de todo el pipeline)
// 2. Logging de requests (antes de cualquier procesamiento)
// 3. Autenticacion/Autorizacion (si aplica)
// 4. Routing y endpoints
// ============================================================================

// 1. Middleware de manejo global de excepciones
// POR QUE VA PRIMERO:
// Debe ser el primer middleware para poder capturar cualquier excepcion
// que ocurra en los middlewares posteriores o en los controladores.
app.UseGlobalExceptionHandler();

// 2. Middleware de logging de requests
// POR QUE VA SEGUNDO:
// Registra cada peticion entrante DESPUES de que el manejador de errores
// este activo, asegurando que incluso los errores sean loggeados.
app.UseRequestLogging();

// Configure Swagger (solo en desarrollo)
// POR QUE SOLO EN DESARROLLO:
// Swagger expone informacion detallada de la API que podria ser
// aprovechada por atacantes en produccion.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JSONPlaceholder API v1");
        c.RoutePrefix = string.Empty;
    });
}

// Map controllers (endpoints)
app.MapControllers();

// Log de inicio de la aplicacion
// POR QUE LOGGEAR EL INICIO:
// Permite verificar en los logs que la aplicacion inicio correctamente
// y con que configuracion.
app.Logger.LogInformation(
    "API iniciada. Ambiente: {Environment}, URL Base JSONPlaceholder: {BaseUrl}",
    app.Environment.EnvironmentName,
    jsonPlaceholderBaseUrl);

app.Run();
