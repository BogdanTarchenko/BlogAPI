using FluentValidation;
using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Validators;

public class UserRegisterModelValidator : AbstractValidator<UserRegisterModel>
{
    public UserRegisterModelValidator() 
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
        
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(1000).WithMessage("Full name must be less than 1000 characters");
        
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gender must be a valid gender");
        
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Invalid phone number");
    }
}