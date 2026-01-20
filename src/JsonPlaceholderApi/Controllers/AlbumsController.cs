using Microsoft.AspNetCore.Mvc;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlbumsController : ControllerBase
{
    private readonly IAlbumService _service;

    public AlbumsController(IAlbumService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Album>>> GetAll()
    {
        var albums = await _service.GetAllAsync();
        return Ok(albums);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Album>> GetById(int id)
    {
        var album = await _service.GetByIdAsync(id);
        if (album == null)
            return NotFound();
        return Ok(album);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Album>>> GetByUserId(int userId)
    {
        var albums = await _service.GetByUserIdAsync(userId);
        return Ok(albums);
    }

    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IEnumerable<Photo>>> GetPhotos(int id)
    {
        var photos = await _service.GetPhotosAsync(id);
        return Ok(photos);
    }

    [HttpPost]
    public async Task<ActionResult<Album>> Create(Album album)
    {
        var created = await _service.CreateAsync(album);
        if (created == null)
            return BadRequest();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Album>> Update(int id, Album album)
    {
        var updated = await _service.UpdateAsync(id, album);
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
