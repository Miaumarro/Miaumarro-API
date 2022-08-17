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
public sealed class AuthenticationController : ControllerBase
{
    private readonly AuthenticationService _service;

    public AuthenticationController(AuthenticationService service)
        => _service = service;

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserAuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<UserAuthenticationResponse, ErrorResponse>>> RegisterAsync([FromBody] UserAuthenticationRequest user)
        => await _service.LoginUserAsync(user);
}