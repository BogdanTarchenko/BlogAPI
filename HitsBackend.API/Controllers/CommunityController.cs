using System.Security.Claims;
using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HitsBackend.Controllers;

[ApiController]
[Route("api/community")]
public class CommunityController : ControllerBase
{
    private readonly ICommunityService _communityService;

    public CommunityController(ICommunityService communityService)
    {
        _communityService = communityService;
    }

    /// <summary>
    /// Get community list
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CommunityDto>>> GetAllCommunities()
    {
        var communities = await _communityService.GetAllCommunitiesAsync();
        return Ok(communities);
    }
    
    /// <summary>
    /// Subscribe a user to the community
    /// </summary>
    [HttpPost("{id}/subscribe")]
    [Authorize]
    public async Task<IActionResult> Subscribe(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        await _communityService.SubscribeAsync(id, Guid.Parse(userId));
        return Ok();
    }
    
    /// <summary>
    /// Unsubscribe a user from the community
    /// </summary>
    [HttpDelete("{id}/unsubscribe")]
    [Authorize]
    public async Task<IActionResult> Unsubscribe(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        await _communityService.UnsubscribeAsync(id, Guid.Parse(userId));
        return Ok();
    }

    /// <summary>
    /// Get user's community list (with the greatest user's role in the community)
    /// </summary>
    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<List<CommunityUserDto>>> GetUserCommunities()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var communities = await _communityService.GetUserCommunitiesAsync(Guid.Parse(userId));
        return Ok(communities);
    }

    /// <summary>
    /// Get information about community
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CommunityFullDto>> GetCommunityById(Guid id)
    {
        var community = await _communityService.GetCommunityByIdAsync(id);
        if (community == null)
        {
            return NotFound();
        }
        return Ok(community);
    }

    /// <summary>
    /// Create a post in the specified community
    /// </summary>
    [HttpPost("{id}/post")]
    [Authorize]
    public async Task<IActionResult> CreatePostInCommunity(Guid id, [FromBody] CreatePostDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        
        var postId = await _communityService.CreatePostInCommunityAsync(id, Guid.Parse(userId), dto);
        return Ok(postId);
    }
} 