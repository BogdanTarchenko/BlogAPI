using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;

    public AddressService(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task<List<SearchAddressModel>> SearchAddressesAsync(long? parentObjectId, string query)
    {
        return await _addressRepository.SearchAddressesAsync(parentObjectId, query);
    }

    public async Task<List<SearchAddressModel>> GetChainAsync(Guid objectGuid)
    {
        return await _addressRepository.GetChainAsync(objectGuid);
    }
}