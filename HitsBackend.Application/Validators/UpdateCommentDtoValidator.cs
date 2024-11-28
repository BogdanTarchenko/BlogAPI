using FluentValidation;
using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Validators;

public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
{
    public UpdateCommentDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(1000).WithMessage("Content must be less than 1000 characters");
    }
}