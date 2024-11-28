namespace HitsBackend.Application.Common.Models;

public record CreateCommentDto(
    string Content,
    Guid? ParentId
);