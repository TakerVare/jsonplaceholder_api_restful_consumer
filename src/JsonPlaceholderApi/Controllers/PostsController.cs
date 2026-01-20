using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JsonPlaceholderApi.Auth;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Models.Auth;
using JsonPlaceholderApi.Services.Interfaces;

namespace JsonPlaceholderApi.Controllers;

/// <summary>
/// Controlador para operaciones CRUD de Posts.
///
/// POLITICA DE SEGURIDAD:
/// - GET (lectura): Publico, no requiere autenticacion
/// - POST, PUT, DELETE (escritura): Requiere autenticacion y permisos especificos
///
/// POR QUE ENDPOINTS DE LECTURA PUBLICOS:
/// 1. JSONPlaceholder es una API publica de prueba
/// 2. Los datos son ficticios, no hay informacion sensible
/// 3. Facilita pruebas y demostraciones sin necesidad de login
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _service;

    public PostsController(IPostService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtiene todos los posts.
    /// </summary>
    [HttpGet]
    [AllowAnonymous] // Lectura publica
    public async Task<ActionResult<IEnumerable<Post>>> GetAll()
    {
        var posts = await _service.GetAllAsync();
        return Ok(posts);
    }

    /// <summary>
    /// Obtiene un post por su ID.
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous] // Lectura publica
    public async Task<ActionResult<Post>> GetById(int id)
    {
        var post = await _service.GetByIdAsync(id);
        if (post == null)
            return NotFound();
        return Ok(post);
    }

    /// <summary>
    /// Obtiene todos los posts de un usuario.
    /// </summary>
    [HttpGet("user/{userId}")]
    [AllowAnonymous] // Lectura publica
    public async Task<ActionResult<IEnumerable<Post>>> GetByUserId(int userId)
    {
        var posts = await _service.GetByUserIdAsync(userId);
        return Ok(posts);
    }

    /// <summary>
    /// Obtiene los comentarios de un post.
    /// </summary>
    [HttpGet("{id}/comments")]
    [AllowAnonymous] // Lectura publica
    public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int id)
    {
        var comments = await _service.GetCommentsAsync(id);
        return Ok(comments);
    }

    /// <summary>
    /// Crea un nuevo post.
    /// Requiere permiso: Posts.Create
    /// </summary>
    [HttpPost]
    [RequirePermission(Resources.Posts, Operations.Create)]
    // POR QUE RequirePermission EN LUGAR DE [Authorize]:
    // 1. Mas granular: verifica permiso especifico, no solo autenticacion
    // 2. Un usuario autenticado sin permiso Posts.Create sera rechazado
    // 3. Permite roles con diferentes niveles de acceso
    public async Task<ActionResult<Post>> Create(Post post)
    {
        var created = await _service.CreateAsync(post);
        if (created == null)
            return BadRequest();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Actualiza un post existente.
    /// Requiere permiso: Posts.Update
    /// </summary>
    [HttpPut("{id}")]
    [RequirePermission(Resources.Posts, Operations.Update)]
    public async Task<ActionResult<Post>> Update(int id, Post post)
    {
        var updated = await _service.UpdateAsync(id, post);
        if (updated == null)
            return NotFound();
        return Ok(updated);
    }

    /// <summary>
    /// Elimina un post.
    /// Requiere permiso: Posts.Delete
    /// </summary>
    /// <remarks>
    /// Solo usuarios con rol Admin pueden eliminar posts.
    /// Usuarios con rol Editor NO tienen este permiso.
    /// </remarks>
    [HttpDelete("{id}")]
    [RequirePermission(Resources.Posts, Operations.Delete)]
    // POR QUE DELETE REQUIERE PERMISO ESPECIAL:
    // La eliminacion es una operacion destructiva e irreversible.
    // Solo administradores deberian poder eliminar contenido.
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
