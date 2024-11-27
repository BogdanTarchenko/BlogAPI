namespace HitsBackend.Application.Common.Models;

public record CreatePostDto
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int ReadingTime { get; init; }
    public string? Image { get; init; }
    public Guid? AddressId { get; init; }
    public required List<Guid> Tags { get; init; }
} 