using Kotz.Extensions;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Encrypt = BCrypt.Net.BCrypt;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to users.
/// </summary>
public sealed class LoginService
{
    private readonly IConfiguration _config;
    private readonly MiauDbContext _db;

    public LoginService(MiauDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    /// <summary>
    /// Authenticates a user.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<LoginUserResponse, ErrorResponse>>> LoginUserAsync(LoginUserRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(dbUser => dbUser.Email == request.Email);

        return (user is null || !Encrypt.Verify(request.Password, user.HashedPassword))
            ? new BadRequestObjectResult(new ErrorResponse("Login ou senha inválida"))
            : new OkObjectResult(new LoginUserResponse(GetToken(user)));
    }

    private string GetToken(UserEntity user)
    {
        // Generate the claims (for authorization)
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
        };

        // Make it so each UserPermissions the user has is a role claim
        foreach (var value in user.Permissions.ToValues())
            claims.Add(new Claim(ClaimTypes.Role, value.ToString()));

        // Generate the token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_config.GetValue<byte[]>("Jwt:Key")), SecurityAlgorithms.HmacSha256Signature),
            Expires = DateTime.UtcNow.AddHours(2),
            Subject = new ClaimsIdentity(claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}