using HitsBackend.Domain.Enums;

namespace HitsBackend.Application.Common.Models;

public record UserDto
{
    public required Guid Id { get; init; }
    public required DateTime CreateTime { get; init; }
    public required string FullName { get; init; }
    public DateTime? BirthDate { get; init; }
    public required Gender Gender { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
}