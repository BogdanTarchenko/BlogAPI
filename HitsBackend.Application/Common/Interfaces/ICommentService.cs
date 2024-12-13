using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Common.Interfaces;

public interface ICommentService
{
    Task DeleteAsync(Guid id, Guid userId);
    Task UpdateAsync(Guid id, UpdateCommentDto dto, Guid userId);
    Task<List<CommentDto>> GetCommentTreeAsync(Guid id, Guid? userId);
    Task AddCommentToPostAsync(Guid postId, CreateCommentDto dto, Guid authorId);
} 