using JsonPlaceholderApi.Models;

namespace JsonPlaceholderApi.Repositories.Interfaces;

public interface ITodoRepository : IRepository<TodoItem>
{
    Task<IEnumerable<TodoItem>> GetByUserIdAsync(int userId);
    Task<IEnumerable<TodoItem>> GetCompletedAsync();
    Task<IEnumerable<TodoItem>> GetPendingAsync();
}
