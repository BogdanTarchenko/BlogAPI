using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Common.Interfaces;

public interface IUserService
{
    Task<TokenResponse> RegisterAsync(UserRegisterModel model);
    Task<TokenResponse> LoginAsync(LoginCredentials credentials);
    Task LogoutAsync(string token);
    Task<UserDto> GetProfileAsync(Guid userId);
    Task UpdateProfileAsync(Guid userId, UserEditModel model);
}