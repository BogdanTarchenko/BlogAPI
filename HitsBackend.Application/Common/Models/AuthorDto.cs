using HitsBackend.Domain.Enums;

namespace HitsBackend.Application.Common.Models;

public record AuthorDto
{
    public required string FullName { get; init; }
    public DateTime? BirthDate { get; init; }
    public required Gender Gender { get; init; }
    public required int Posts { get; init; }
    public required int Likes { get; init; }
    public required DateTime Created { get; init; }
}