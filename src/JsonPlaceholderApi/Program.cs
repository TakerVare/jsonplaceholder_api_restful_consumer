using JsonPlaceholderApi.Repositories;
using JsonPlaceholderApi.Repositories.Interfaces;
using JsonPlaceholderApi.Services;
using JsonPlaceholderApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

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

// Configure HttpClient for JSONPlaceholder API
builder.Services.AddHttpClient<IPostRepository, PostRepository>(client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});

builder.Services.AddHttpClient<ICommentRepository, CommentRepository>(client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});

builder.Services.AddHttpClient<IUserRepository, UserRepository>(client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});

builder.Services.AddHttpClient<IAlbumRepository, AlbumRepository>(client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});

builder.Services.AddHttpClient<IPhotoRepository, PhotoRepository>(client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});

builder.Services.AddHttpClient<ITodoRepository, TodoRepository>(client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});

// Register Services
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JSONPlaceholder API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.MapControllers();

app.Run();
