using HitsBackend.Application.Common.Exceptions;
using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Entities;

namespace HitsBackend.Application.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;

    public CommentService(ICommentRepository commentRepository, IPostRepository postRepository)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null)
        {
            throw new NotFoundException(nameof(Comment), id);
        }

        if (comment.AuthorId != userId)
        {
            throw new ForbiddenException("You do not have permission to delete this comment.");
        }

        if (comment.IsDeleted)
        {
            throw new ConflictException("This comment has already been deleted.");
        }

        if (comment.Replies.Any())
        {
            comment.IsDeleted = true;
            comment.Content = null;
            comment.DeleteDate = DateTime.UtcNow;
            await _commentRepository.UpdateAsync(comment);
        }
        else
        {
            await _commentRepository.DeleteAsync(id);
        }
    }

    public async Task UpdateAsync(Guid id, UpdateCommentDto dto, Guid userId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null)
        {
            throw new NotFoundException(nameof(Comment), id);
        }

        if (comment.AuthorId != userId)
        {
            throw new ForbiddenException("You do not have permission to update this comment.");
        }

        if (comment.IsDeleted)
        {
            throw new ConflictException("Cannot edit a deleted comment.");
        }


        comment.Content = dto.Content;
        comment.ModifiedDate = DateTime.UtcNow;

        await _commentRepository.UpdateAsync(comment);
    }

    public async Task<List<CommentDto>> GetCommentTreeAsync(Guid id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null)
        {
            throw new NotFoundException(nameof(Comment), id);
        }

        var replies = await _commentRepository.GetRepliesAsync(id);

        var commentDtos = new List<CommentDto>();
        foreach (var reply in replies)
        {
            var subReplies = await _commentRepository.GetRepliesAsync(reply.Id);
            commentDtos.Add(new CommentDto(
                Id: reply.Id,
                CreateTime: reply.CreateTime,
                Content: reply.Content,
                ModifiedDate: reply.ModifiedDate,
                DeleteDate: reply.DeleteDate,
                AuthorId: reply.AuthorId,
                Author: reply.Author.FullName,
                SubComments: subReplies.Count
            ));
        }

        return commentDtos;
    }

    public async Task AddCommentToPostAsync(Guid postId, CreateCommentDto dto, Guid authorId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
        {
            throw new NotFoundException(nameof(Post), postId);
        }

        if (dto.ParentId.HasValue)
        {
            var parentComment = await _commentRepository.GetByIdAsync(dto.ParentId.Value);
            if (parentComment == null)
            {
                throw new NotFoundException(nameof(Comment), dto.ParentId.Value);
            }
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = dto.Content,
            CreateTime = DateTime.UtcNow,
            PostId = postId,
            ParentCommentId = dto.ParentId,
            AuthorId = authorId
        };

        await _commentRepository.CreateAsync(comment);
    }
} 