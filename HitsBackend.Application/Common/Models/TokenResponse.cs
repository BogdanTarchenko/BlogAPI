namespace HitsBackend.Application.Common.Models;

public record TokenResponse
{
    public required string Token { get; init; }
}