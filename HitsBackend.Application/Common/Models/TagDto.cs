namespace HitsBackend.Application.Common.Models;

public record TagDto
{
    public required Guid Id { get; init; }
    public required DateTime CreateTime { get; init; }
    public required string Name { get; init; }
}