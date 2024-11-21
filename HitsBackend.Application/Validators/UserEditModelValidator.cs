using FluentValidation;
using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Validators;

public class UserEditModelValidator : AbstractValidator<UserEditModel>
{
    public UserEditModelValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");
        
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(1000).WithMessage("Full name must be less than 1000 characters");
        
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gender must be a valid gender");
        
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Phone number must match format: +7 (XXX) XXX-XX-XX");
    }
}