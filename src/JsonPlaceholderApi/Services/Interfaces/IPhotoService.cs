using JsonPlaceholderApi.Models;

namespace JsonPlaceholderApi.Services.Interfaces;

public interface IPhotoService
{
    Task<IEnumerable<Photo>> GetAllAsync();
    Task<Photo?> GetByIdAsync(int id);
    Task<IEnumerable<Photo>> GetByAlbumIdAsync(int albumId);
    Task<Photo?> CreateAsync(Photo photo);
    Task<Photo?> UpdateAsync(int id, Photo photo);
    Task<bool> DeleteAsync(int id);
}
