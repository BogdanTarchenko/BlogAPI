using System.Security.Claims;
using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HitsBackend.Controllers;

[ApiController]
[Route("api/account")]
[Tags("Users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Register new user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<TokenResponse>> Register(UserRegisterModel model)
    {
        var result = await _userService.RegisterAsync(model);
        return Ok(result);
    }
    
    /// <summary>
    /// Log in to the system
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login(LoginCredentials credentials)
    {
        var result = await _userService.LoginAsync(credentials);
        return Ok(result);
    }

    /// <summary>
    /// Log out system user
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _userService.LogoutAsync();
        return Ok();
    }
    
    /// <summary>
    /// Get user profile
    /// </summary>
    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();
        
        var result = await _userService.GetProfileAsync(Guid.Parse(userId));
        return Ok(result);
    }
    
    /// <summary>
    /// Edit user profile
    /// </summary>
    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UserEditModel model)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();
        
        await _userService.UpdateProfileAsync(Guid.Parse(userId), model);
        return Ok();
    }
}