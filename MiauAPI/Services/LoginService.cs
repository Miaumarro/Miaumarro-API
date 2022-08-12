using Kotz.Extensions;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauDatabase;
using MiauDatabase.Entities;
using MiauDatabase.Enums;
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
    public async Task<ActionResult<OneOf<UserAuthenticationResponse, ErrorResponse>>> LoginUserAsync(LoginUserRequest request)
    {
        var expireAt = DateTime.UtcNow.AddDays(7);
        var user = await _db.Users
            .Where(x => x.Cpf == request.Cpf || x.Email == request.Email)
            .Select(x => new UserEntity()
            {
                // Select only the database columns we need
                // to generate a token and the response
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Permissions = x.Permissions,
                HashedPassword = x.HashedPassword
            })
            .FirstOrDefaultAsync();

        return (user is not null && Encrypt.Verify(request.Password, user.HashedPassword))
            ? new OkObjectResult(new UserAuthenticationResponse(user.Id, GenerateSessionToken(user, expireAt), expireAt))
            : new BadRequestObjectResult(new ErrorResponse("CPF or e-mail were not found, or password is invalid."));
    }

    private string GenerateSessionToken(UserEntity dbUser, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(dbUser.Name))
            throw new ArgumentException("Username cannot be null or whitespace.", nameof(dbUser));
        else if (string.IsNullOrWhiteSpace(dbUser.Email))
            throw new ArgumentException("User e-mail cannot be null or whitespace.", nameof(dbUser));

        return GenerateSessionToken(dbUser.Name, dbUser.Email, dbUser.Permissions, expiresAt);
    }

    private string GenerateSessionToken(string name, string email, UserPermissions permissions, DateTime expiresAt)
    {
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Token must expire in the future.", nameof(expiresAt));

        // Generate the claims (for authorization)
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email),
        };

        // Make it so each UserPermissions the user has is a role claim
        foreach (var permission in permissions.ToValues())
            claims.Add(new Claim(ClaimTypes.Role, permission.ToString()));

        // Generate the token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_config.GetValue<byte[]>("Jwt:Key")), SecurityAlgorithms.HmacSha256Signature),
            Expires = expiresAt,
            Subject = new ClaimsIdentity(claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}