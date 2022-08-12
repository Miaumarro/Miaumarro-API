using MiauAPI.Common;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf;

namespace MiauAPI.Controllers;

[ApiController]
[Route(ApiConstants.MainEndpoint)]
public sealed class LoginController : ControllerBase
{
    private readonly LoginService _service;

    public LoginController(LoginService service)
        => _service = service;

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserAuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<UserAuthenticationResponse, ErrorResponse>>> RegisterAsync([FromBody] LoginUserRequest user)
        => await _service.LoginUserAsync(user);
}