using MiauAPI.Common;
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

    [HttpGet]
    [ProducesResponseType(typeof(GetPetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetPetResponse, ErrorResponse>>> GetAsync([FromQuery] PetParameters petParameters)
    {
        var petsPaged = await _service.GetPetAsync(petParameters);

        if (petsPaged.Result is OkObjectResult response && response.Value is GetPetResponse petResponse)
        {
            var metadata = new
            {
                petResponse.Pets.TotalCount,
                petResponse.Pets.PageSize,
                petResponse.Pets.CurrentPage,
                petResponse.Pets.TotalPages,
                petResponse.Pets.HasNext,
                petResponse.Pets.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        }

        return petsPaged;
    }

    [HttpGet("detail")]
    [ProducesResponseType(typeof(GetPetByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetPetByIdResponse, ErrorResponse>>> GetByIdAsync([FromQuery] int id)
    => await _service.GetPetByIdAsync(id);

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedPetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedPetResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedPetRequest pet)
        => await _service.CreatePetAsync(pet, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeleteByIdAsync([FromQuery] int id)
        => await _service.DeletePetByIdAsync(id);

    [HttpPut("update")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse>>> UpdateByIdAsync([FromBody] UpdatePetRequest pet)
        => await _service.UpdatePetByIdAsync(pet);
}