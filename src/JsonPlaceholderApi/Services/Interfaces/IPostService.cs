using JsonPlaceholderApi.Models;

namespace JsonPlaceholderApi.Services.Interfaces;

public interface IPostService
{
    Task<IEnumerable<Post>> GetAllAsync();
    Task<Post?> GetByIdAsync(int id);
    Task<IEnumerable<Post>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Comment>> GetCommentsAsync(int postId);
    Task<Post?> CreateAsync(Post post);
    Task<Post?> UpdateAsync(int id, Post post);
    Task<bool> DeleteAsync(int id);
}
