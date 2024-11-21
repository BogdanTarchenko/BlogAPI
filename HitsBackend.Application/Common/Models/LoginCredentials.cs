namespace HitsBackend.Application.Common.Models;

public record LoginCredentials
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}