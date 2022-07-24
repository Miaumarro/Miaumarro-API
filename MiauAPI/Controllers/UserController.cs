using MiauAPI.Common;
using MiauAPI.Extensions;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiauAPI.Controllers;

[ApiController]
[Route(ApiConstants.MainEndpoint)]
public sealed class UserController : ControllerBase
{
    private readonly UserService _service;

    public UserController(UserService service)
        => _service = service;

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreateUserRequest), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegisterAsync([FromBody] CreateUserRequest user)
    {
        var response = await _service.CreateUserAsync(user);
        return response.ToActionResult();
    }
}