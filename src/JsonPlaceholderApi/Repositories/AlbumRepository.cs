using System.Net.Http.Json;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Repositories.Interfaces;

namespace JsonPlaceholderApi.Repositories;

public class AlbumRepository : IAlbumRepository
{
    private readonly HttpClient _httpClient;
    private const string Endpoint = "albums";

    public AlbumRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Album>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Album>>(Endpoint) ?? [];
    }

    public async Task<Album?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Album>($"{Endpoint}/{id}");
    }

    public async Task<Album?> CreateAsync(Album entity)
    {
        var response = await _httpClient.PostAsJsonAsync(Endpoint, entity);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Album>()
            : null;
    }

    public async Task<Album?> UpdateAsync(int id, Album entity)
    {
        var response = await _httpClient.PutAsJsonAsync($"{Endpoint}/{id}", entity);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Album>()
            : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{Endpoint}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<Album>> GetByUserIdAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Album>>($"{Endpoint}?userId={userId}") ?? [];
    }

    public async Task<IEnumerable<Photo>> GetPhotosAsync(int albumId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Photo>>($"{Endpoint}/{albumId}/photos") ?? [];
    }
}
