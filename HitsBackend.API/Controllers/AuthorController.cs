using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace HitsBackend.Controllers;

[ApiController]
[Route("api/author")]
[Tags("Author")]
public class AuthorController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    /// <summary>
    /// Get author list
    /// </summary>
    [HttpGet("list")]
    public async Task<ActionResult<List<AuthorDto>>> GetAll()
    {
        return await _authorService.GetAllAsync();
    }
} 