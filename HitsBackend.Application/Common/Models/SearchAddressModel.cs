using HitsBackend.Domain.Enums;

namespace HitsBackend.Application.Common.Models;

public record SearchAddressModel(
    long ObjectId,
    Guid ObjectGuid,
    string? Text,
    GarAddressLevel ObjectLevel,
    string? ObjectLevelText
);