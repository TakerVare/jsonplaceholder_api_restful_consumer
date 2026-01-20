using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Repositories.Interfaces;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _repository;

    public CommentService(ICommentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Comment>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Comment?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Comment>> GetByPostIdAsync(int postId)
    {
        return await _repository.GetByPostIdAsync(postId);
    }

    public async Task<Comment?> CreateAsync(Comment comment)
    {
        return await _repository.CreateAsync(comment);
    }

    public async Task<Comment?> UpdateAsync(int id, Comment comment)
    {
        return await _repository.UpdateAsync(id, comment);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
