using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
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
} 