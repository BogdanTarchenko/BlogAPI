using System.Security.Claims;
using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HitsBackend.Controllers;

[ApiController]
[Route("api/post")]
[Tags("Post")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// Get a list of available posts
    /// </summary>
    [HttpGet]
    [Authorize]
    [AllowAnonymous]
    public async Task<ActionResult<PostPagedListDto>> GetAll(
        [FromQuery] List<Guid>? tags,
        [FromQuery] string? author,
        [FromQuery] int? min,
        [FromQuery] int? max,
        [FromQuery] PostSorting sorting = PostSorting.CreateDesc,
        [FromQuery] bool onlyMyCommunities = false,
        [FromQuery] int page = 1,
        [FromQuery] int size = 5)
    {
        Guid? userId = null;
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(userIdClaim))
        {
            userId = Guid.Parse(userIdClaim);
        }

        return await _postService.GetAllAsync(
            tags, author, min, max, sorting, 
            onlyMyCommunities, page, size, userId);
    }

    /// <summary>
    /// Create a personal user post
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guid>> Create([FromBody] CreatePostDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var postId = await _postService.CreateAsync(Guid.Parse(userId), dto);
        return Ok(postId);
    }

    /// <summary>
    /// Get information about concrete post
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [AllowAnonymous]
    public async Task<ActionResult<PostFullDto>> GetById(Guid id)
    {
        Guid? userId = null;
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(userIdClaim))
        {
            userId = Guid.Parse(userIdClaim);
        }

        var post = await _postService.GetByIdAsync(id, userId);
        return Ok(post);
    }

    /// <summary>
    /// Add like to concrete post
    /// </summary>
    [HttpPost("{postId:guid}/like")]
    [Authorize]
    public async Task<IActionResult> AddLike(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        await _postService.AddLikeAsync(id, Guid.Parse(userId));
        return Ok();
    }

    /// <summary>
    /// Delete like from concrete post
    /// </summary>
    [HttpDelete("{postId:guid}/like")]
    [Authorize]
    public async Task<IActionResult> RemoveLike(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        await _postService.RemoveLikeAsync(id, Guid.Parse(userId));
        return Ok();
    }
}