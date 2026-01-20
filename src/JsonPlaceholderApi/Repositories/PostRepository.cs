using System.Net.Http.Json;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Repositories.Interfaces;

namespace JsonPlaceholderApi.Repositories;

public class PostRepository : IPostRepository
{
    private readonly HttpClient _httpClient;
    private const string Endpoint = "posts";

    public PostRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Post>>(Endpoint) ?? [];
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Post>($"{Endpoint}/{id}");
    }

    public async Task<Post?> CreateAsync(Post entity)
    {
        var response = await _httpClient.PostAsJsonAsync(Endpoint, entity);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Post>()
            : null;
    }

    public async Task<Post?> UpdateAsync(int id, Post entity)
    {
        var response = await _httpClient.PutAsJsonAsync($"{Endpoint}/{id}", entity);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Post>()
            : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{Endpoint}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<Post>> GetByUserIdAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Post>>($"{Endpoint}?userId={userId}") ?? [];
    }

    public async Task<IEnumerable<Comment>> GetCommentsAsync(int postId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Comment>>($"{Endpoint}/{postId}/comments") ?? [];
    }
}
