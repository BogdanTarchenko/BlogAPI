using FluentValidation;
using HitsBackend.Application.Common.Exceptions;
using HitsBackend.Application.Common.Helpers;
using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Mappings;
using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Entities;
using ValidationException = HitsBackend.Application.Common.Exceptions.ValidationException;

namespace HitsBackend.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IBannedTokenRepository _bannedTokenRepository;
    
    private readonly IValidator<UserRegisterModel> _userRegisterValidator;
    private readonly IValidator<LoginCredentials> _loginCredentialsValidator;
    private readonly IValidator<UserEditModel> _userEditValidator;

    public UserService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IValidator<UserRegisterModel> userRegisterValidator,
        IValidator<LoginCredentials> loginCredentialsValidator,
        IValidator<UserEditModel> userEditValidator,
        IBannedTokenRepository bannedTokenRepository)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _userRegisterValidator = userRegisterValidator;
        _loginCredentialsValidator = loginCredentialsValidator;
        _userEditValidator = userEditValidator;
        _bannedTokenRepository = bannedTokenRepository;
    }
    public async Task<TokenResponse> RegisterAsync(UserRegisterModel model)
    {
        var validationResult = await _userRegisterValidator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var existingUser = await _userRepository.GetByEmailAsync(model.Email);
        if (existingUser != null)
        {
            throw new ConflictException("User with this email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow,
            Email = model.Email,
            FullName = model.FullName,
            PasswordHash = PasswordHasher.HashPassword(model.Password),
            Gender = model.Gender,
            BirthDate = model.BirthDate,
            PhoneNumber = model.PhoneNumber
        };

        await _userRepository.CreateAsync(user);
        
        return new TokenResponse
        {
            Token = _jwtService.GenerateToken(user)
        };
    }

    public async Task<TokenResponse> LoginAsync(LoginCredentials credentials)
    {
        var validationResult = await _loginCredentialsValidator.ValidateAsync(credentials);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var user = await _userRepository.GetByEmailAsync(credentials.Email);
        if (user == null)
        {
            throw new ValidationException("Invalid email or password");
        }

        if (!PasswordHasher.VerifyPassword(credentials.Password, user.PasswordHash))
        {
            throw new ValidationException("Invalid email or password");
        }

        return new TokenResponse
        {
            Token = _jwtService.GenerateToken(user)
        };
    }

    public async Task LogoutAsync(string token)
    {
        await _bannedTokenRepository.AddAsync(token);
    }

    public async Task<UserDto> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        return UserMapper.ToDto(user);
    }

    public async Task UpdateProfileAsync(Guid userId, UserEditModel model)
    {
        var validationResult = await _userEditValidator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        if (model.Email != user.Email)
        {
            var existingUser = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                throw new ConflictException("Email is already taken");
            }
        }

        user.Email = model.Email;
        user.FullName = model.FullName;
        user.Gender = model.Gender;
        user.BirthDate = model.BirthDate;
        user.PhoneNumber = model.PhoneNumber;

        await _userRepository.UpdateAsync(user);
    }
}