using MiauAPI.Common;
using MiauAPI.Extensions;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using System.Text.Json;

namespace MiauAPI.Controllers;

[ApiController, Route(ApiConstants.MainEndpoint)]
public sealed class AddressController : ControllerBase
{
    private readonly AddressService _service;

    public AddressController(AddressService service)
        => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<AddressObject[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<PagedResponse<AddressObject[]>, None>>> GetAsync([FromQuery] AddressParameters addressParameters)
    {
        var adressesPaged = await _service.GetAddressesAsync(addressParameters);

        if (adressesPaged.Result.TryUnwrap<PagedResponse>(out var response))
        {
            var metadata = new
            {
                response.PageNumber,
                response.PageSize,
                response.PreviousCount,
                response.NextCount,
                response.Amount
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        }

        return adressesPaged;
    }

    [HttpGet("details")]
    [ProducesResponseType(typeof(AddressObject), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<AddressObject, None>>> GetAppointmentAsync([FromQuery] GetAddressRequest request)
        => await _service.GetAddressByIdsAsync(request);

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedAddressResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<CreatedAddressResponse, ErrorResponse, None>>> RegisterAsync([FromBody] CreatedAddressRequest address)
        => await _service.CreateAddressAsync(address, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteByIdAsync([FromQuery] int id)
        => await _service.DeleteAddressAsync(id);

    [HttpPut("update")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse, None>>> UpdateByIdAsync([FromBody] UpdateAddressRequest address)
        => await _service.UpdateAddressAsync(address);
}