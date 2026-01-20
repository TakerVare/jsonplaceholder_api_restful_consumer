using JsonPlaceholderApi.Models;

namespace JsonPlaceholderApi.Services.Interfaces;

public interface ITodoService
{
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(int id);
    Task<IEnumerable<TodoItem>> GetByUserIdAsync(int userId);
    Task<IEnumerable<TodoItem>> GetCompletedAsync();
    Task<IEnumerable<TodoItem>> GetPendingAsync();
    Task<TodoItem?> CreateAsync(TodoItem todo);
    Task<TodoItem?> UpdateAsync(int id, TodoItem todo);
    Task<bool> DeleteAsync(int id);
}
