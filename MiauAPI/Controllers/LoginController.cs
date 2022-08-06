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


    /*
    private IConfiguration _config;
    private MiauDbContext _miauDbContext;

    public LoginController(IConfiguration config, MiauDbContext miauDbContext)
    {
        _config = config;
        _miauDbContext = miauDbContext;
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login([FromBody] LoginUserRequest userLogin)
    {
        var user = Authenticate(userLogin);

        if (user != null)
        {
            var token = Generate(user);
            return Ok(token);
        }

        return NotFound("User not found");
    }

    private string Generate(CreatedUserRequest user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
               //new Claim(ClaimTypes.GivenName, user.GivenName),
                new Claim(ClaimTypes.Surname, user.Surname),
               // new Claim(ClaimTypes.Role, user.Role)
            };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
          _config["Jwt:Audience"],
          claims,
          expires: DateTime.Now.AddMinutes(15),
          signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private MiauDbContext Authenticate(LoginUserRequest userLogin)
    {
        var currentUser = MiauDbContext(o => o.Email.ToLower() == userLogin.Email.ToLower() && o.Password == userLogin.Password);

        if (currentUser != null)
        {
            return currentUser;
        }

        return null;
    }*/
}
