using MiauAPI.Common;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using System.Text.Json;

namespace MiauAPI.Controllers;

[ApiController]
[Route(ApiConstants.MainEndpoint)]
public sealed class UserController : ControllerBase
{
    private readonly UserService _service;

    public UserController(UserService service)
        => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetUserResponse, ErrorResponse>>> GetAsync([FromQuery] UserParameters userParameters)
    {
        var usersPaged = await _service.GetUserAsync(userParameters);

        if (usersPaged.Result is OkObjectResult response && response.Value is GetUserResponse userResponse)
        {
            var metadata = new
            {
                userResponse.Users.TotalCount,
                userResponse.Users.PageSize,
                userResponse.Users.CurrentPage,
                userResponse.Users.TotalPages,
                userResponse.Users.HasNext,
                userResponse.Users.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        }

        return usersPaged;
    }

    [HttpGet("detail")]
    [ProducesResponseType(typeof(GetUserByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetUserByIdResponse, ErrorResponse>>> GetByIdAsync([FromQuery] int id)
        => await _service.GetUserByIdAsync(id);

    [HttpPost("create")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserAuthenticationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<UserAuthenticationResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedUserRequest user)
        => await _service.CreateUserAsync(user, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeleteByIdAsync([FromQuery] int id)
        => await _service.DeleteUserByIdAsync(id);

    [HttpPut("update")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse>>> UpdateByIdAsync([FromBody] UpdateUserRequest user)
        => await _service.UpdateUserByIdAsync(user);
}