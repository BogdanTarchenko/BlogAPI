using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace HitsBackend.Controllers;

[ApiController]
[Route("api/address")]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<SearchAddressModel>>> Search([FromQuery] long? parentObjectId,
        [FromQuery] string? query)
    {
        var results = await _addressService.SearchAddressesAsync(parentObjectId, query);
        return Ok(results);
    }

    [HttpGet("chain")]
    public async Task<ActionResult<List<SearchAddressModel>>> GetChain([FromQuery] Guid objectGuid)
    {
        var chain = await _addressService.GetChainAsync(objectGuid);
        return Ok(chain);
    }
}