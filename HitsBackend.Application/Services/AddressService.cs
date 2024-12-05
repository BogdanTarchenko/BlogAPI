using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Entities;
using HitsBackend.Application.Common.Exceptions;

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
        bool addressExists = await _addressRepository.AddressExistsAsync(objectGuid);
        if (!addressExists)
        {
            throw new ValidationException("The specified address does not exist.");
        }

        return await _addressRepository.GetChainAsync(objectGuid);
    }
}