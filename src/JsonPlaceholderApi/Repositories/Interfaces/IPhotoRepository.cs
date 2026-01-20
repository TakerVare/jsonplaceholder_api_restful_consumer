using JsonPlaceholderApi.Models;

namespace JsonPlaceholderApi.Repositories.Interfaces;

public interface IPhotoRepository : IRepository<Photo>
{
    Task<IEnumerable<Photo>> GetByAlbumIdAsync(int albumId);
}
