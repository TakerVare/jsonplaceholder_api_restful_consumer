using System.Net.Http.Json;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Repositories.Interfaces;

namespace JsonPlaceholderApi.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly HttpClient _httpClient;
    private const string Endpoint = "todos";

    public TodoRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<TodoItem>>(Endpoint) ?? [];
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<TodoItem>($"{Endpoint}/{id}");
    }

    public async Task<TodoItem?> CreateAsync(TodoItem entity)
    {
        var response = await _httpClient.PostAsJsonAsync(Endpoint, entity);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<TodoItem>()
            : null;
    }

    public async Task<TodoItem?> UpdateAsync(int id, TodoItem entity)
    {
        var response = await _httpClient.PutAsJsonAsync($"{Endpoint}/{id}", entity);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<TodoItem>()
            : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{Endpoint}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<TodoItem>>($"{Endpoint}?userId={userId}") ?? [];
    }

    public async Task<IEnumerable<TodoItem>> GetCompletedAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<TodoItem>>($"{Endpoint}?completed=true") ?? [];
    }

    public async Task<IEnumerable<TodoItem>> GetPendingAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<TodoItem>>($"{Endpoint}?completed=false") ?? [];
    }
}
