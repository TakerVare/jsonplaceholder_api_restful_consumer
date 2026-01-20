using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Repositories.Interfaces;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _repository;

    public PostService(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Post>> GetByUserIdAsync(int userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<Comment>> GetCommentsAsync(int postId)
    {
        return await _repository.GetCommentsAsync(postId);
    }

    public async Task<Post?> CreateAsync(Post post)
    {
        return await _repository.CreateAsync(post);
    }

    public async Task<Post?> UpdateAsync(int id, Post post)
    {
        return await _repository.UpdateAsync(id, post);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
