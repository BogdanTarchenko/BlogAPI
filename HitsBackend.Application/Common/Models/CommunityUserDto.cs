using HitsBackend.Domain.Enums;

namespace HitsBackend.Application.Common.Models;

public record CommunityUserDto
{
    public required Guid UserId { get; init; }
    public required Guid CommunityId { get; init; }
    public required CommunityRole Role { get; init; }
}