using JsonPlaceholderApi.Models;

namespace JsonPlaceholderApi.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<Post>> GetPostsAsync(int userId);
    Task<IEnumerable<Album>> GetAlbumsAsync(int userId);
    Task<IEnumerable<TodoItem>> GetTodosAsync(int userId);
    Task<User?> CreateAsync(User user);
    Task<User?> UpdateAsync(int id, User user);
    Task<bool> DeleteAsync(int id);
}
