using Microsoft.AspNetCore.Mvc;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _service;

    public CommentsController(ICommentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Comment>>> GetAll()
    {
        var comments = await _service.GetAllAsync();
        return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Comment>> GetById(int id)
    {
        var comment = await _service.GetByIdAsync(id);
        if (comment == null)
            return NotFound();
        return Ok(comment);
    }

    [HttpGet("post/{postId}")]
    public async Task<ActionResult<IEnumerable<Comment>>> GetByPostId(int postId)
    {
        var comments = await _service.GetByPostIdAsync(postId);
        return Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult<Comment>> Create(Comment comment)
    {
        var created = await _service.CreateAsync(comment);
        if (created == null)
            return BadRequest();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Comment>> Update(int id, Comment comment)
    {
        var updated = await _service.UpdateAsync(id, comment);
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
