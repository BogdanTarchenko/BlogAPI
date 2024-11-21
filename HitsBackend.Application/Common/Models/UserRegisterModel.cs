using HitsBackend.Domain.Enums;

namespace HitsBackend.Application.Common.Models;

public record UserRegisterModel
{
    public required string FullName { get; init; }
    public required string Password { get; init; }
    public required string Email { get; init; }
    public required Gender Gender { get; init; }
    public DateTime? BirthDate { get; init; }
    public string? PhoneNumber { get; init; }
}