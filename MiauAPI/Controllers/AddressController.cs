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

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedAddressResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedAddressResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedAddressRequest address)
        => await _service.CreateAddressAsync(address, base.Request.Path.Value!);

}