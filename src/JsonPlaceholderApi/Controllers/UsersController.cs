using Microsoft.AspNetCore.Mvc;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll()
    {
        var users = await _service.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _service.GetByIdAsync(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpGet("{id}/posts")]
    public async Task<ActionResult<IEnumerable<Post>>> GetPosts(int id)
    {
        var posts = await _service.GetPostsAsync(id);
        return Ok(posts);
    }

    [HttpGet("{id}/albums")]
    public async Task<ActionResult<IEnumerable<Album>>> GetAlbums(int id)
    {
        var albums = await _service.GetAlbumsAsync(id);
        return Ok(albums);
    }

    [HttpGet("{id}/todos")]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos(int id)
    {
        var todos = await _service.GetTodosAsync(id);
        return Ok(todos);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Create(User user)
    {
        var created = await _service.CreateAsync(user);
        if (created == null)
            return BadRequest();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<User>> Update(int id, User user)
    {
        var updated = await _service.UpdateAsync(id, user);
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
