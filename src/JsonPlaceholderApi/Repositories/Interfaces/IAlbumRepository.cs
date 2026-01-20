using JsonPlaceholderApi.Models;

namespace JsonPlaceholderApi.Repositories.Interfaces;

public interface IAlbumRepository : IRepository<Album>
{
    Task<IEnumerable<Album>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Photo>> GetPhotosAsync(int albumId);
}
