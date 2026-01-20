using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Repositories.Interfaces;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Services;

public class AlbumService : IAlbumService
{
    private readonly IAlbumRepository _repository;

    public AlbumService(IAlbumRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Album>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Album?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Album>> GetByUserIdAsync(int userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<Photo>> GetPhotosAsync(int albumId)
    {
        return await _repository.GetPhotosAsync(albumId);
    }

    public async Task<Album?> CreateAsync(Album album)
    {
        return await _repository.CreateAsync(album);
    }

    public async Task<Album?> UpdateAsync(int id, Album album)
    {
        return await _repository.UpdateAsync(id, album);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
