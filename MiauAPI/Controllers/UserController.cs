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
public sealed class UserController : ControllerBase
{
    private readonly UserService _service;

    public UserController(UserService service)
        => _service = service;

    [HttpGet("users")]
    [Authorize(Roles = $"{nameof(UserPermissions.Clerk)},{nameof(UserPermissions.Administrator)}")]
    [ProducesResponseType(typeof(PagedResponse<UserObject[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<PagedResponse<UserObject[]>, ErrorResponse>>> GetUsersAsync([FromQuery] UserParameters userParameters)
    {
        var actionResult = await _service.GetUsersAsync(userParameters);

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
    [ProducesResponseType(typeof(UserObject), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<UserObject, None>>> GetUserAsync(int userId)
        => await _service.GetUserByIdAsync(userId);

    [HttpPost("create")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserAuthenticationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<UserAuthenticationResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedUserRequest user)
        => await _service.RegisterUserAsync(user, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeleteAsync([FromQuery] UserParameters userParameters)
        => await _service.DeleteUsersAsync(userParameters);

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<None, ErrorResponse>>> UpdateAsync([FromBody] UpdateUserRequest user)
        => await _service.UpdateUserAsync(user);

    [HttpPut("updatePassword")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse>>> UpdatePasswordAsync([FromBody] UpdateUserPasswordRequest user)
        => await _service.UpdateUserPasswordAsync(user);
}