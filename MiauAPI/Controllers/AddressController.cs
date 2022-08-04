using MiauAPI.Common;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using System.Text.Json;

namespace MiauAPI.Controllers;

[ApiController]
[Route(ApiConstants.MainEndpoint)]
public sealed class AddressController : ControllerBase
{
    private readonly AddressService _service;

    public AddressController(AddressService service)
        => _service = service;

    [HttpGet()]
    [ProducesResponseType(typeof(GetAddressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetAddressResponse, ErrorResponse>>> GetAsync([FromQuery] AddressParameters addressParameters)
    {
        var adressesPaged = await _service.GetAddressAsync(addressParameters);

        if (adressesPaged.Result is OkObjectResult response && response.Value is GetAddressResponse addressResponse)
        {
            var metadata = new
            {
                addressResponse.Addresses.TotalCount,
                addressResponse.Addresses.PageSize,
                addressResponse.Addresses.CurrentPage,
                addressResponse.Addresses.TotalPages,
                addressResponse.Addresses.HasNext,
                addressResponse.Addresses.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

        }
        return adressesPaged;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedAddressResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedAddressResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedAddressRequest address)
        => await _service.CreateAddressAsync(address, base.Request.Path.Value!);

}