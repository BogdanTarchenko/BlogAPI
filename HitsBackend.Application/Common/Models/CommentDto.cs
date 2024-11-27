namespace HitsBackend.Application.Common.Models;

public record CommentDto(
    Guid Id,
    DateTime CreateTime,
    string Content,
    DateTime? ModifiedDate,
    DateTime? DeleteDate,
    Guid AuthorId,
    string Author,
    int SubComments
);