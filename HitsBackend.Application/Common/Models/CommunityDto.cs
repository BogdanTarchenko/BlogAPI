namespace HitsBackend.Application.Common.Models;

public record CommunityDto
{
    public required Guid Id { get; init; }
    public required DateTime CreateTime { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required bool IsClosed { get; init; }
    public required int SubscribersCount { get; init; }
}