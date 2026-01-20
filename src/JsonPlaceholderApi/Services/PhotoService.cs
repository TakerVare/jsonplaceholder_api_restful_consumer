using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Repositories.Interfaces;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Services;

public class PhotoService : IPhotoService
{
    private readonly IPhotoRepository _repository;

    public PhotoService(IPhotoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Photo>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Photo?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Photo>> GetByAlbumIdAsync(int albumId)
    {
        return await _repository.GetByAlbumIdAsync(albumId);
    }

    public async Task<Photo?> CreateAsync(Photo photo)
    {
        return await _repository.CreateAsync(photo);
    }

    public async Task<Photo?> UpdateAsync(int id, Photo photo)
    {
        return await _repository.UpdateAsync(id, photo);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
