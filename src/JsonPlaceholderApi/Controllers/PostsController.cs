using Microsoft.AspNetCore.Mvc;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _service;

    public PostsController(IPostService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> GetAll()
    {
        var posts = await _service.GetAllAsync();
        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Post>> GetById(int id)
    {
        var post = await _service.GetByIdAsync(id);
        if (post == null)
            return NotFound();
        return Ok(post);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Post>>> GetByUserId(int userId)
    {
        var posts = await _service.GetByUserIdAsync(userId);
        return Ok(posts);
    }

    [HttpGet("{id}/comments")]
    public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int id)
    {
        var comments = await _service.GetCommentsAsync(id);
        return Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult<Post>> Create(Post post)
    {
        var created = await _service.CreateAsync(post);
        if (created == null)
            return BadRequest();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Post>> Update(int id, Post post)
    {
        var updated = await _service.UpdateAsync(id, post);
        if (updated == null)
            return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
