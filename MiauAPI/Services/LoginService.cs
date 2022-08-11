using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;
using MiauDatabase.Entities;
using MiauDatabase.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    public LoginService(MiauDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <remarks>If the request contains invalid data or the CPF/e-mail are already registered, the operation fails.</remarks>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<LoginUserResponse, ErrorResponse>>> LoginUserAsync(LoginUserRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(dbUser => dbUser.Email == request.Email);

        return (user is null || !Encrypt.Verify(request.Password, user.HashedPassword))
            ? new BadRequestObjectResult(new ErrorResponse("Login ou senha inválida"))
            : new OkObjectResult(new LoginUserResponse(GetToken(user)));
    }

    private string GetToken(UserEntity user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_config.GetValue<byte[]>("Jwt:Key")), SecurityAlgorithms.HmacSha256Signature),
            Expires = DateTime.UtcNow.AddHours(2),
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, $"{Enum.GetValues<UserPermissions>().Last(x => x is not UserPermissions.Blocked && user.Permissions.HasFlag(x))}"),
            })
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}