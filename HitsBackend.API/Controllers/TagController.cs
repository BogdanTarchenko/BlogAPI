using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace HitsBackend.Controllers;

[ApiController]
[Route("api/tag")]
[Tags("Tag")]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    /// <summary>
    /// Get tag list
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetAll()
    {
        return await _tagService.GetAllAsync();
    }
}