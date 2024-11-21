using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Services;

public class UserService : IUserService
{
    public Task<TokenResponse> RegisterAsync(UserRegisterModel model)
    {
        throw new NotImplementedException();
    }

    public Task<TokenResponse> LoginAsync(LoginCredentials credentials)
    {
        throw new NotImplementedException();
    }

    public Task LogoutAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> GetProfileAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateProfileAsync(Guid userId, UserEditModel model)
    {
        throw new NotImplementedException();
    }
}