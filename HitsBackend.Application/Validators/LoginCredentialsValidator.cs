using FluentValidation;
using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Validators;

public class LoginCredentialsValidator : AbstractValidator<LoginCredentials>
{
    public LoginCredentialsValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}