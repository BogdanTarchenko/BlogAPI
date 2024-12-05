using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Common.Interfaces;

public interface IAddressRepository
{
    Task<List<SearchAddressModel>> SearchAddressesAsync(long? parentObjectId, string query);
    Task<List<SearchAddressModel>> GetChainAsync(Guid objectGuid);
}