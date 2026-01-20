using System.Net.Http.Json;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Repositories.Interfaces;

namespace JsonPlaceholderApi.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly HttpClient _httpClient;
    private const string Endpoint = "photos";

    public PhotoRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Photo>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Photo>>(Endpoint) ?? [];
    }

    public async Task<Photo?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Photo>($"{Endpoint}/{id}");
    }

    public async Task<Photo?> CreateAsync(Photo entity)
    {
        var response = await _httpClient.PostAsJsonAsync(Endpoint, entity);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Photo>()
            : null;
    }

    public async Task<Photo?> UpdateAsync(int id, Photo entity)
    {
        var response = await _httpClient.PutAsJsonAsync($"{Endpoint}/{id}", entity);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Photo>()
            : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{Endpoint}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<Photo>> GetByAlbumIdAsync(int albumId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Photo>>($"{Endpoint}?albumId={albumId}") ?? [];
    }
}
