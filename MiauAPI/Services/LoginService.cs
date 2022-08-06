using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Encrypt = BCrypt.Net.BCrypt;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to users.
/// </summary>
public sealed class LoginService
{
    private IConfiguration _config;
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<LoginUserRequest> _validator;

    public LoginService(MiauDbContext db, IRequestValidator<LoginUserRequest> validator, IConfiguration config)
    {
        _db = db;
        _validator = validator;
        _config = config;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <remarks>If the request contains invalid data or the CPF/e-mail are already registered, the operation fails.</remarks>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<LoginUserResponse, ErrorResponse>>> LoginUserAsync(LoginUserRequest request, string location)
        {


        var user = _db.Users.SingleOrDefault(dbUser => dbUser.Email == request.Email);

        // verify password
        if (user == null || !Encrypt.Verify(request.Password, user.HashedPassword))
        {
            return new BadRequestObjectResult(new ErrorResponse("Login ou senha invalida"));
        }


        // TODO: handle authentication properly
        return new CreatedResult(location, new LoginUserResponse(GetToken(user)));

    }
    // Get token 
    private string GetToken(UserEntity user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config.GetSection("Jwt:key").Value);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                   //new Claim(ClaimTypes.Role, user.Role)
                                   

            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}