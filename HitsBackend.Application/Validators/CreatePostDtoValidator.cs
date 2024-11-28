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
            .MaximumLength(1000).WithMessage("Title must be less than 1000 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(5).WithMessage("Description must be at least 5 characters")
            .MaximumLength(5000).WithMessage("Description must be less than 5000 characters");

        RuleFor(x => x.ReadingTime)
            .GreaterThan(0).WithMessage("Reading time must be greater than 0");

        RuleFor(x => x.Image)
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.Image))
            .WithMessage("Image must be a valid URL")
            .MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Image))
            .WithMessage("Image URL must be less than 1000 characters");

        RuleFor(x => x.Tags)
            .NotEmpty().WithMessage("At least one tag is required")
            .ForEach(tag => tag.Must(BeAValidGuid).WithMessage("Each tag must be a valid GUID"));

        RuleFor(x => x.AddressId)
            .Must(BeAValidGuid).When(x => x.AddressId.HasValue)
            .WithMessage("AddressId must be a valid GUID");
    }

    private static bool BeAValidUrl(string? url)
    {
        return url != null && Uri.TryCreate(url, UriKind.Absolute, out _);
    }

    private static bool BeAValidGuid(Guid? id)
    {
        return id == null || id != Guid.Empty;
    }

    private static bool BeAValidGuid(Guid id)
    {
        return id != Guid.Empty;
    }
}