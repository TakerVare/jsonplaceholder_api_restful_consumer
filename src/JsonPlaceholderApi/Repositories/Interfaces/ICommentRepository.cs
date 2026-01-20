using JsonPlaceholderApi.Models;

namespace JsonPlaceholderApi.Repositories.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByPostIdAsync(int postId);
}
