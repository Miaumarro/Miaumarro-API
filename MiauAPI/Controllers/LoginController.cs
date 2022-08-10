using MiauAPI.Common;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using MiauDatabase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiauAPI.Controllers;

[ApiController]
[Route(ApiConstants.MainEndpoint)]
public sealed class LoginController : ControllerBase
{

    private readonly LoginService _service;

    public LoginController(LoginService service)
        => _service = service;

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<LoginUserResponse, ErrorResponse>>> RegisterAsync([FromBody] LoginUserRequest user)
        => await _service.LoginUserAsync(user, base.Request.Path.Value!);

}
