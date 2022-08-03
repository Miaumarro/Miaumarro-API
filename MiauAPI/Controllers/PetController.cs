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
public sealed class PetController : ControllerBase
{
    private readonly PetService _service;

    public PetController(PetService service)
        => _service = service;

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedPetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedPetResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedPetRequest pet)
        => await _service.CreatePetAsync(pet, base.Request.Path.Value!);

}