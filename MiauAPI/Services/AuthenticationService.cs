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
public sealed class AuthenticationService
{
    private readonly IConfiguration _config;
    private readonly MiauDbContext _db;

    /// <summary>
    /// Gets the expiration time for a new session token.
    /// </summary>
    public DateTime TokenExpirationTime
        => DateTime.UtcNow.AddDays(7);

    public AuthenticationService(MiauDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    /// <summary>
    /// Authenticates a user.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// Occurs when <paramref name="request"/> or <see cref="UserAuthenticationRequest.Password"/>
    /// are <see langword="null"/>.
    /// </exception>
    public async Task<ActionResult<OneOf<UserAuthenticationResponse, ErrorResponse>>> LoginUserAsync(UserAuthenticationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentNullException.ThrowIfNull(request.Password, nameof(request.Password));

        var expireAt = TokenExpirationTime;
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

    /// <summary>
    /// Generates a session token for the specified user.
    /// </summary>
    /// <param name="dbUser">The user to generate the token for.</param>
    /// <param name="expiresAt">The date and time the token should expire.</param>
    /// <returns>A session token.</returns>
    /// <exception cref="ArgumentException">
    /// Occurs when <paramref name="dbUser"/> contains invalid data or when
    /// <paramref name="expiresAt"/> is less than the current time.
    /// </exception>
    public string GenerateSessionToken(UserEntity dbUser, DateTime expiresAt)
        => GenerateSessionToken(dbUser.Name, dbUser.Email, dbUser.Permissions, expiresAt);

    /// <summary>
    /// Generates a session token for the specified name and e-mail.
    /// </summary>
    /// <param name="name">The name of the user.</param>
    /// <param name="email">The e-mail of the user.</param>
    /// <param name="permissions">The permissions of the user.</param>
    /// <param name="expiresAt">The date and time the token should expire.</param>
    /// <returns>A session token.</returns>
    /// <exception cref="ArgumentException">
    /// Occurs when <paramref name="name"/> or <paramref name="email"/> are <see langword="null"/> or whitespace
    /// or when <paramref name="expiresAt"/> is less than the current time.
    /// </exception>
    public string GenerateSessionToken(string name, string email, UserPermissions permissions, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Username cannot be null or whitespace.", nameof(name));
        else if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("User e-mail cannot be null or whitespace.", nameof(email));
        else if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Token must expire in the future.", nameof(expiresAt));

        // Generate the claims (for authorization)
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email),
        };

        // Make it so each UserPermissions the user has is a role claim
        foreach (var permission in permissions.ToValues().When(x => x.Count() is not 1, x => x.Where(y => y is not UserPermissions.Blocked)))
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