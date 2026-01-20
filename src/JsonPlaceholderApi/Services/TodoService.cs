using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Repositories.Interfaces;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(int userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<TodoItem>> GetCompletedAsync()
    {
        return await _repository.GetCompletedAsync();
    }

    public async Task<IEnumerable<TodoItem>> GetPendingAsync()
    {
        return await _repository.GetPendingAsync();
    }

    public async Task<TodoItem?> CreateAsync(TodoItem todo)
    {
        return await _repository.CreateAsync(todo);
    }

    public async Task<TodoItem?> UpdateAsync(int id, TodoItem todo)
    {
        return await _repository.UpdateAsync(id, todo);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
