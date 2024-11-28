using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HitsBackend.Controllers;

[ApiController]
[Route("api")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }
    
    /// <summary>
    /// Get all nested comments(replies)
    /// </summary>
    [HttpGet("comment/{id:guid}/tree")]
    public async Task<ActionResult<List<CommentDto>>> GetCommentTree(Guid id)
    {
        var comments = await _commentService.GetCommentTreeAsync(id);
        return Ok(comments);
    }

    /// <summary>
    /// Add a comment to a concrete post
    /// </summary>
    [HttpPost("post/{id:guid}/comment")]
    [Authorize]
    public async Task<IActionResult> AddCommentToPost(Guid id, CreateCommentDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var authorId = Guid.Parse(userIdClaim);
        await _commentService.AddCommentToPostAsync(id, dto, authorId);
        return Ok();
    }
    
    /// <summary>
    /// Edit concrete comment
    /// </summary>
    [HttpPut("comment/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, UpdateCommentDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim);
        await _commentService.UpdateAsync(id, dto, userId);
        return Ok();
    }

    /// <summary>
    /// Delete concrete comment
    /// </summary>
    [HttpDelete("comment/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim);
        await _commentService.DeleteAsync(id, userId);
        return Ok();
    }
}