using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using JsonPlaceholderApi.Auth;
using JsonPlaceholderApi.Middleware;
using JsonPlaceholderApi.Models.Auth;
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

// ----------------------------------------------------------------------------
// CONFIGURACION DE JWT
// ----------------------------------------------------------------------------
// POR QUE JWT (JSON Web Tokens):
// 1. Stateless: El servidor no necesita almacenar sesiones, el token contiene
//    toda la informacion necesaria para autenticar al usuario.
// 2. Escalable: Funciona bien en arquitecturas distribuidas y microservicios.
// 3. Seguro: Firmado digitalmente, no puede ser modificado sin invalidarse.
// 4. Estandar: Ampliamente adoptado, muchas librerias y herramientas disponibles.
// ----------------------------------------------------------------------------

// Cargar configuracion de JWT desde appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings no configurado en appsettings.json");

// Configurar autenticacion JWT
builder.Services.AddAuthentication(options =>
{
    // POR QUE ESTABLECER ESQUEMAS POR DEFECTO:
    // Evita tener que especificar el esquema en cada [Authorize]
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // POR QUE VALIDAR ISSUER:
        // Asegura que el token fue emitido por nuestra API
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,

        // POR QUE VALIDAR AUDIENCE:
        // Asegura que el token fue creado para esta aplicacion
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,

        // POR QUE VALIDAR LIFETIME:
        // Rechaza tokens expirados automaticamente
        ValidateLifetime = true,

        // POR QUE VALIDAR SIGNING KEY:
        // Verifica que el token no fue modificado
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

        // POR QUE ClockSkew = Zero:
        // Por defecto hay 5 minutos de tolerancia, lo eliminamos para
        // que la expiracion sea exacta
        ClockSkew = TimeSpan.Zero
    };

    // POR QUE MANEJAR EVENTOS:
    // Permite logging personalizado de errores de autenticacion
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();

            logger.LogWarning("Autenticacion JWT fallida: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();

            var userId = context.Principal?.FindFirst("sub")?.Value;
            logger.LogDebug("Token JWT validado para usuario: {UserId}", userId);
            return Task.CompletedTask;
        }
    };
});

// ----------------------------------------------------------------------------
// CONFIGURACION DE AUTORIZACION
// ----------------------------------------------------------------------------
// POR QUE POLICY-BASED AUTHORIZATION:
// 1. Mas flexible que solo roles
// 2. Permite permisos granulares (Posts.Create, Posts.Delete, etc.)
// 3. Reutilizable y testeable
// ----------------------------------------------------------------------------

builder.Services.AddAuthorization(options =>
{
    // Registrar todas las politicas de permisos
    options.AddPermissionPolicies();
});

// Registrar el handler de autorizacion
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

// ----------------------------------------------------------------------------
// CONFIGURACION DE SWAGGER CON JWT
// ----------------------------------------------------------------------------
// POR QUE CONFIGURAR JWT EN SWAGGER:
// Permite probar endpoints protegidos directamente desde la UI de Swagger
// sin necesidad de herramientas externas como Postman.
// ----------------------------------------------------------------------------

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JSONPlaceholder API Consumer",
        Version = "v1",
        Description = "API RESTful que consume datos de JSONPlaceholder con autenticacion JWT"
    });

    // Configurar esquema de seguridad JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT. Ejemplo: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ----------------------------------------------------------------------------
// CONFIGURACION DE HTTPCLIENT
// ----------------------------------------------------------------------------

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

builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<ITodoService, TodoService>();

// Servicio de autenticacion
// POR QUE SINGLETON PARA AuthService:
// Mantiene el almacen de usuarios en memoria persistente durante
// toda la vida de la aplicacion.
builder.Services.AddSingleton<IAuthService, AuthService>();

var app = builder.Build();

// ============================================================================
// CONFIGURACION DEL PIPELINE DE MIDDLEWARES
// ============================================================================
// ORDEN ACTUALIZADO CON AUTENTICACION:
// 1. Manejo de excepciones
// 2. Logging de requests
// 3. Swagger (desarrollo)
// 4. Autenticacion (valida tokens)
// 5. Autorizacion (verifica permisos)
// 6. Endpoints
// ============================================================================

// 1. Middleware de manejo global de excepciones
app.UseGlobalExceptionHandler();

// 2. Middleware de logging de requests
app.UseRequestLogging();

// 3. Configure Swagger (solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JSONPlaceholder API v1");
        c.RoutePrefix = string.Empty;
    });
}

// 4. Middleware de autenticacion
// POR QUE ANTES DE AUTORIZACION:
// La autenticacion identifica QUIEN es el usuario (valida el token)
// La autorizacion verifica QUE puede hacer (verifica permisos)
// Primero necesitamos saber quien es antes de verificar que puede hacer.
app.UseAuthentication();

// 5. Middleware de autorizacion
app.UseAuthorization();

// 6. Map controllers (endpoints)
app.MapControllers();

// Log de inicio de la aplicacion
app.Logger.LogInformation(
    "API iniciada con JWT. Ambiente: {Environment}, URL Base JSONPlaceholder: {BaseUrl}",
    app.Environment.EnvironmentName,
    jsonPlaceholderBaseUrl);

app.Run();
