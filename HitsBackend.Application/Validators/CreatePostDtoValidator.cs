using FluentValidation;
using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Validators;

public class CreatePostDtoValidator : AbstractValidator<CreatePostDto>
{
    public CreatePostDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters")
            .MaximumLength(1000).WithMessage("Title must not exceed 1000 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(5).WithMessage("Description must be at least 5 characters")
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters");

        RuleFor(x => x.ReadingTime)
            .GreaterThan(0).WithMessage("Reading time must be greater than 0");

        RuleFor(x => x.Image)
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.Image))
            .WithMessage("Image must be a valid URL")
            .MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Image))
            .WithMessage("Image URL must not exceed 1000 characters");

        RuleFor(x => x.Tags)
            .NotEmpty().WithMessage("At least one tag is required");
    }

    private static bool BeAValidUrl(string? url)
    {
        return url != null && Uri.TryCreate(url, UriKind.Absolute, out _);
    }
} 