using HitsBackend.Domain.Enums;

namespace HitsBackend.Application.Common.Models;

public record UserRegisterModel(
    string FullName,
    string Password,
    string Email,
    Gender Gender,
    DateTime? BirthDate,
    string? PhoneNumber);