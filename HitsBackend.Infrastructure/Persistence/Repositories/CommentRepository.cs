using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;

    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await _context.Comments
            .Include(c => c.Author)
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Comment>> GetByPostIdAsync(Guid postId)
    {
        return await _context.Comments
            .Where(c => c.PostId == postId && !c.IsDeleted)
            .Include(c => c.Author)
            .ToListAsync();
    }

    public async Task CreateAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var comment = await _context.Comments
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment != null)
        {
            if (comment.Replies.Any())
            {
                comment.IsDeleted = true;
                comment.Content = null;
            }
            else
            {
                _context.Comments.Remove(comment);
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Comment>> GetRepliesAsync(Guid parentId)
    {
        return await _context.Comments
            .Where(c => c.ParentCommentId == parentId && !c.IsDeleted)
            .Include(c => c.Author)
            .ToListAsync();
    }
}