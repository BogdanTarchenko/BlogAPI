using FluentValidation;
using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Validators;

public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(1000).WithMessage("Content must be less than 1000 characters");

        RuleFor(x => x.ParentId)
            .Must(BeAValidGuid).WithMessage("ParentId must be a valid GUID");
    }

    private bool BeAValidGuid(Guid? parentId)
    {
        return parentId == null || parentId != Guid.Empty;
    }
}