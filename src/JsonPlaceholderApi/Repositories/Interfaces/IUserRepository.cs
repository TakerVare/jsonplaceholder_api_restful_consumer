using JsonPlaceholderApi.Models;

namespace JsonPlaceholderApi.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<IEnumerable<Post>> GetPostsAsync(int userId);
    Task<IEnumerable<Album>> GetAlbumsAsync(int userId);
    Task<IEnumerable<TodoItem>> GetTodosAsync(int userId);
}
