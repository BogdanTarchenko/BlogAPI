using HitsBackend.Application.Common.Exceptions;
using HitsBackend.Application.Common.Interfaces;

namespace HitsBackend.Middleware;

public class TokenValidationMiddleware : IMiddleware
{
    private readonly IBannedTokenRepository _bannedTokenRepository;

    public TokenValidationMiddleware(IBannedTokenRepository bannedTokenRepository)
    {
        _bannedTokenRepository = bannedTokenRepository;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last();

        if (token != null && await _bannedTokenRepository.IsTokenBannedAsync(token))
        {
            throw new UnauthorizedException("Token is no longer valid");
        }

        await next(context);
    }
} 