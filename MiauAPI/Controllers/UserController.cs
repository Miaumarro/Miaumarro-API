using MiauAPI.Common;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using Microsoft.AspNetCore.Mvc;
using OneOf;

namespace MiauAPI.Controllers;

[ApiController]
[Route(ApiConstants.MainEndpoint)]
public sealed class UserController : ControllerBase
{
    private readonly UserService _service;

    public UserController(UserService service)
        => _service = service;

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedUserResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedUserRequest user)
        => await _service.CreateUserAsync(user, base.Request.Path.Value!);
}