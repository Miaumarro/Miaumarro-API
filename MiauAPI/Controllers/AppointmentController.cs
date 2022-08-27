using MiauAPI.Common;
using MiauAPI.Extensions;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using MiauDatabase.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using System.Text.Json;

namespace MiauAPI.Controllers;

[ApiController, Route(ApiConstants.MainEndpoint)]
public sealed class AppointmentController : ControllerBase
{
    private readonly AppointmentService _service;

    public AppointmentController(AppointmentService service)
        => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<AppointmentObject[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<PagedResponse<AppointmentObject[]>, None>>> GetAsync([FromQuery] AppointmentParameters appointmentsParameters)
    {
        var appointmentsPaged = await _service.GetAppointmentsAsync(appointmentsParameters);

        if (appointmentsPaged.Result.TryUnwrap<PagedResponse>(out var response))
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

        return appointmentsPaged;
    }

    [HttpGet("details")]
    [ProducesResponseType(typeof(AppointmentObject), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<AppointmentObject, None>>> GetAppointmentAsync([FromQuery] GetAppointmentRequest request)
        => await _service.GetAppointmentByIdsAsync(request);

    [HttpPost("create")]
    [Authorize(Roles = nameof(UserPermissions.Customer))]
    [ProducesResponseType(typeof(CreatedAppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<CreatedAppointmentResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedAppointmentRequest appointment)
        => await _service.CreateAppointmentAsync(appointment, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteByIdAsync([FromQuery] int id)
        => await _service.DeleteAppointmentAsync(id);

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<None, ErrorResponse>>> UpdateByIdAsync([FromBody] UpdateAppointmentRequest appointment)
        => await _service.UpdateAppointmentByIdAsync(appointment);
}