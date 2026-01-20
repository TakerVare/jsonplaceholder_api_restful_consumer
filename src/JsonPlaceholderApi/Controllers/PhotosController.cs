using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JsonPlaceholderApi.Auth;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Models.Auth;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhotosController : ControllerBase
{
    private readonly IPhotoService _service;

    public PhotosController(IPhotoService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Photo>>> GetAll()
    {
        var photos = await _service.GetAllAsync();
        return Ok(photos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Photo>> GetById(int id)
    {
        var photo = await _service.GetByIdAsync(id);
        if (photo == null)
            return NotFound();
        return Ok(photo);
    }

    [HttpGet("album/{albumId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Photo>>> GetByAlbumId(int albumId)
    {
        var photos = await _service.GetByAlbumIdAsync(albumId);
        return Ok(photos);
    }

    [HttpPost]
    [RequirePermission(Resources.Photos, Operations.Create)]
    public async Task<ActionResult<Photo>> Create(Photo photo)
    {
        var created = await _service.CreateAsync(photo);
        if (created == null)
            return BadRequest();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [RequirePermission(Resources.Photos, Operations.Update)]
    public async Task<ActionResult<Photo>> Update(int id, Photo photo)
    {
        var updated = await _service.UpdateAsync(id, photo);
        if (updated == null)
            return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [RequirePermission(Resources.Photos, Operations.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
