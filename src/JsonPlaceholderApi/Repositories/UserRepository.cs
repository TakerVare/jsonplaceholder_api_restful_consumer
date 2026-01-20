using System.Net.Http.Json;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Repositories.Interfaces;

namespace JsonPlaceholderApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly HttpClient _httpClient;
    private const string Endpoint = "users";

    public UserRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<User>>(Endpoint) ?? [];
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<User>($"{Endpoint}/{id}");
    }

    public async Task<User?> CreateAsync(User entity)
    {
        var response = await _httpClient.PostAsJsonAsync(Endpoint, entity);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<User>()
            : null;
    }

    public async Task<User?> UpdateAsync(int id, User entity)
    {
        var response = await _httpClient.PutAsJsonAsync($"{Endpoint}/{id}", entity);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<User>()
            : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{Endpoint}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<Post>> GetPostsAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Post>>($"{Endpoint}/{userId}/posts") ?? [];
    }

    public async Task<IEnumerable<Album>> GetAlbumsAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Album>>($"{Endpoint}/{userId}/albums") ?? [];
    }

    public async Task<IEnumerable<TodoItem>> GetTodosAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<TodoItem>>($"{Endpoint}/{userId}/todos") ?? [];
    }
}
