using FluentValidation;
using HitsBackend.Application.Common.Exceptions;
using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Entities;
using ValidationException = HitsBackend.Application.Common.Exceptions.ValidationException;

namespace HitsBackend.Application.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;
    
    private readonly IValidator<CreateCommentDto> _createCommentValidator;
    private readonly IValidator<UpdateCommentDto> _updateCommentValidator;

    public CommentService(ICommentRepository commentRepository, IPostRepository postRepository,
        IValidator<CreateCommentDto> createCommentValidator, IValidator<UpdateCommentDto> updateCommentValidator)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _createCommentValidator = createCommentValidator;
        _updateCommentValidator = updateCommentValidator;
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
            comment.Content = string.Empty;
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
        var validationResult = await _updateCommentValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
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

    public async Task<List<CommentDto>> GetCommentTreeAsync(Guid rootCommentId)
    {
        var rootComment = await _commentRepository.GetByIdAsync(rootCommentId);
        if (rootComment == null)
        {
            throw new NotFoundException(nameof(Comment), rootCommentId);
        }

        if (rootComment.ParentCommentId != null)
        {
            throw new ValidationException("Can't get comment tree for a reply comment.");
        }

        var allComments = await _commentRepository.GetAllByPostIdAsync(rootComment.PostId);

        List<CommentDto> BuildFlatList(Guid? parentId)
        {
            var directChildren = allComments
                .Where(c => c.ParentCommentId == parentId)
                .Select(c => new CommentDto(
                    Id: c.Id,
                    CreateTime: c.CreateTime,
                    Content: c.Content,
                    ModifiedDate: c.ModifiedDate,
                    DeleteDate: c.DeleteDate,
                    AuthorId: c.AuthorId,
                    Author: c.Author.FullName,
                    SubComments: allComments.Count(x => x.ParentCommentId == c.Id)
                )).ToList();
            
            var allDescendants = new List<CommentDto>();
            foreach (var comment in directChildren)
            {
                allDescendants.Add(comment);
                allDescendants.AddRange(BuildFlatList(comment.Id));
            }

            return allDescendants;
        }

        return BuildFlatList(rootCommentId);
    }

    public async Task AddCommentToPostAsync(Guid postId, CreateCommentDto dto, Guid authorId)
    {
        var validationResult = await _createCommentValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
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

            if (parentComment.IsDeleted)
            {
                throw new ForbiddenException("Cannot reply to a deleted comment.");
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