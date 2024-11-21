using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Entities;

namespace HitsBackend.Application.Common.Mappings;

public static class UserMapper
{
    public static UserDto ToDto(User user) => new()
    {
        Id = user.Id,
        CreateTime = user.CreateTime,
        Email = user.Email,
        FullName = user.FullName,
        Gender = user.Gender,
        BirthDate = user.BirthDate,
        PhoneNumber = user.PhoneNumber
    };
}