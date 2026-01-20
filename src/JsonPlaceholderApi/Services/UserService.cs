using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Repositories.Interfaces;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Post>> GetPostsAsync(int userId)
    {
        return await _repository.GetPostsAsync(userId);
    }

    public async Task<IEnumerable<Album>> GetAlbumsAsync(int userId)
    {
        return await _repository.GetAlbumsAsync(userId);
    }

    public async Task<IEnumerable<TodoItem>> GetTodosAsync(int userId)
    {
        return await _repository.GetTodosAsync(userId);
    }

    public async Task<User?> CreateAsync(User user)
    {
        return await _repository.CreateAsync(user);
    }

    public async Task<User?> UpdateAsync(int id, User user)
    {
        return await _repository.UpdateAsync(id, user);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
