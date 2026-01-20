using JsonPlaceholderApi.Models;

namespace JsonPlaceholderApi.Services.Interfaces;

public interface IAlbumService
{
    Task<IEnumerable<Album>> GetAllAsync();
    Task<Album?> GetByIdAsync(int id);
    Task<IEnumerable<Album>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Photo>> GetPhotosAsync(int albumId);
    Task<Album?> CreateAsync(Album album);
    Task<Album?> UpdateAsync(int id, Album album);
    Task<bool> DeleteAsync(int id);
}
