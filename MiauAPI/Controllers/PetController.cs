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
public sealed class PetController : ControllerBase
{
    private readonly PetService _service;

    public PetController(PetService service)
        => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PetObject[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<PagedResponse<PetObject[]>, None>>> GetAsync([FromQuery] PetParameters petParameters)
    {
        var actionResult = await _service.GetPetsAsync(petParameters);

        if (actionResult.Result.TryUnwrap<PagedResponse>(out var response))
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

        return actionResult;
    }

    [HttpGet("details")]
    [ProducesResponseType(typeof(PetObject), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<PetObject, None>>> GetByIdAsync([FromQuery] int id)
    => await _service.GetPetByIdAsync(id);

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedPetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<CreatedPetResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedPetRequest pet)
        => await _service.CreatePetAsync(pet, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteByIdAsync([FromQuery] int id)
        => await _service.DeletePetByIdAsync(id);

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<None, ErrorResponse>>> UpdateByIdAsync([FromBody] UpdatePetRequest pet)
        => await _service.UpdatePetAsync(pet);
}