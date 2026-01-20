using System.Net.Http.Json;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Repositories.Interfaces;

namespace JsonPlaceholderApi.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly HttpClient _httpClient;
    private const string Endpoint = "comments";

    public CommentRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Comment>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Comment>>(Endpoint) ?? [];
    }

    public async Task<Comment?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Comment>($"{Endpoint}/{id}");
    }

    public async Task<Comment?> CreateAsync(Comment entity)
    {
        var response = await _httpClient.PostAsJsonAsync(Endpoint, entity);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Comment>()
            : null;
    }

    public async Task<Comment?> UpdateAsync(int id, Comment entity)
    {
        var response = await _httpClient.PutAsJsonAsync($"{Endpoint}/{id}", entity);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Comment>()
            : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{Endpoint}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<Comment>> GetByPostIdAsync(int postId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Comment>>($"{Endpoint}?postId={postId}") ?? [];
    }
}
