using System.Net.Http.Json;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Repositories.Interfaces;

namespace JsonPlaceholderApi.Repositories;

/// <summary>
/// Repositorio para acceso a Posts desde JSONPlaceholder API.
///
/// POR QUE AÑADIR LOGGING EN LOS REPOSITORIOS:
/// 1. Trazabilidad: Permite seguir el flujo de datos desde la peticion HTTP
///    hasta la llamada a la API externa y su respuesta.
/// 2. Diagnostico: Facilita identificar si un problema esta en nuestra API
///    o en el servicio externo (JSONPlaceholder).
/// 3. Metricas: Permite medir tiempos de respuesta de la API externa
///    y detectar degradaciones de rendimiento.
/// 4. Auditoria: Registra todas las operaciones CRUD realizadas contra
///    el servicio externo.
/// </summary>
public class PostRepository : IPostRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PostRepository> _logger;
    private const string Endpoint = "posts";

    public PostRepository(HttpClient httpClient, ILogger<PostRepository> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        // Log de nivel Debug: informacion detallada util durante desarrollo
        // POR QUE USAR DEBUG PARA INICIO DE OPERACION:
        // En produccion normalmente se filtran logs Debug, pero en desarrollo
        // ayudan a entender el flujo de ejecucion sin saturar los logs.
        _logger.LogDebug("Obteniendo todos los posts desde JSONPlaceholder");

        var posts = await _httpClient.GetFromJsonAsync<IEnumerable<Post>>(Endpoint) ?? [];

        // Log informativo con cantidad de resultados
        _logger.LogInformation("Se obtuvieron {Count} posts desde JSONPlaceholder", posts.Count());

        return posts;
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        _logger.LogDebug("Buscando post con Id: {PostId}", id);

        var post = await _httpClient.GetFromJsonAsync<Post>($"{Endpoint}/{id}");

        if (post == null)
        {
            // POR QUE LOGGEAR CUANDO NO SE ENCUENTRA:
            // Ayuda a identificar si hay problemas con IDs invalidos
            // o si el recurso fue eliminado en la fuente.
            _logger.LogWarning("Post con Id: {PostId} no encontrado en JSONPlaceholder", id);
        }

        return post;
    }

    public async Task<Post?> CreateAsync(Post entity)
    {
        // POR QUE LOGGEAR OPERACIONES DE ESCRITURA:
        // Las operaciones que modifican datos son mas criticas y
        // requieren mayor trazabilidad para auditoria y debugging.
        _logger.LogInformation("Creando nuevo post. UserId: {UserId}, Title: {Title}",
            entity.UserId, entity.Title);

        var response = await _httpClient.PostAsJsonAsync(Endpoint, entity);

        if (response.IsSuccessStatusCode)
        {
            var created = await response.Content.ReadFromJsonAsync<Post>();
            _logger.LogInformation("Post creado exitosamente con Id: {PostId}", created?.Id);
            return created;
        }

        // Log de advertencia cuando la operacion falla
        _logger.LogWarning("Fallo al crear post. StatusCode: {StatusCode}",
            response.StatusCode);
        return null;
    }

    public async Task<Post?> UpdateAsync(int id, Post entity)
    {
        _logger.LogInformation("Actualizando post Id: {PostId}", id);

        var response = await _httpClient.PutAsJsonAsync($"{Endpoint}/{id}", entity);

        if (response.IsSuccessStatusCode)
        {
            var updated = await response.Content.ReadFromJsonAsync<Post>();
            _logger.LogInformation("Post Id: {PostId} actualizado exitosamente", id);
            return updated;
        }

        _logger.LogWarning("Fallo al actualizar post Id: {PostId}. StatusCode: {StatusCode}",
            id, response.StatusCode);
        return null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Eliminando post Id: {PostId}", id);

        var response = await _httpClient.DeleteAsync($"{Endpoint}/{id}");

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Post Id: {PostId} eliminado exitosamente", id);
            return true;
        }

        _logger.LogWarning("Fallo al eliminar post Id: {PostId}. StatusCode: {StatusCode}",
            id, response.StatusCode);
        return false;
    }

    public async Task<IEnumerable<Post>> GetByUserIdAsync(int userId)
    {
        _logger.LogDebug("Obteniendo posts del usuario Id: {UserId}", userId);

        var posts = await _httpClient.GetFromJsonAsync<IEnumerable<Post>>($"{Endpoint}?userId={userId}") ?? [];

        _logger.LogInformation("Se obtuvieron {Count} posts para el usuario Id: {UserId}",
            posts.Count(), userId);

        return posts;
    }

    public async Task<IEnumerable<Comment>> GetCommentsAsync(int postId)
    {
        _logger.LogDebug("Obteniendo comentarios del post Id: {PostId}", postId);

        var comments = await _httpClient.GetFromJsonAsync<IEnumerable<Comment>>($"{Endpoint}/{postId}/comments") ?? [];

        _logger.LogInformation("Se obtuvieron {Count} comentarios para el post Id: {PostId}",
            comments.Count(), postId);

        return comments;
    }
}
