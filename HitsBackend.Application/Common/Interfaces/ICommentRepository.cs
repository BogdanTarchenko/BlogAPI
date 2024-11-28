using HitsBackend.Domain.Entities;

namespace HitsBackend.Application.Common.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(Guid id);
    Task<List<Comment>> GetByPostIdAsync(Guid postId);
    Task<List<Comment>> GetRepliesAsync(Guid parentId);
    Task CreateAsync(Comment comment);
    Task DeleteAsync(Guid id);
    Task UpdateAsync(Comment comment);
} 