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
public sealed class AppointmentController : ControllerBase
{
    private readonly AppointmentService _service;

    public AppointmentController(AppointmentService service)
        => _service = service;


    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedAppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedAppointmentResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedAppointmentRequest appointment)
        => await _service.CreateAppointmentAsync(appointment, base.Request.Path.Value!);

}