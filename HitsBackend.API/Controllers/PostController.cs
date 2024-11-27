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
        return await _postService.GetAllAsync(
            tags, author, min, max, sorting, 
            onlyMyCommunities, page, size);
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
    /// Get post by id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostFullDto>> GetById(Guid id)
    {
        return await _postService.GetByIdAsync(id);
    }
} 