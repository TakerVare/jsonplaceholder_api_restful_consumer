using Microsoft.AspNetCore.Mvc;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _service;

    public TodosController(ITodoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll()
    {
        var todos = await _service.GetAllAsync();
        return Ok(todos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetById(int id)
    {
        var todo = await _service.GetByIdAsync(id);
        if (todo == null)
            return NotFound();
        return Ok(todo);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetByUserId(int userId)
    {
        var todos = await _service.GetByUserIdAsync(userId);
        return Ok(todos);
    }

    [HttpGet("completed")]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetCompleted()
    {
        var todos = await _service.GetCompletedAsync();
        return Ok(todos);
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetPending()
    {
        var todos = await _service.GetPendingAsync();
        return Ok(todos);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create(TodoItem todo)
    {
        var created = await _service.CreateAsync(todo);
        if (created == null)
            return BadRequest();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoItem>> Update(int id, TodoItem todo)
    {
        var updated = await _service.UpdateAsync(id, todo);
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
