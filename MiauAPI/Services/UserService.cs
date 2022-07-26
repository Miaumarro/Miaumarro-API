using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Encrypt = BCrypt.Net.BCrypt;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to users.
/// </summary>
public sealed class UserService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<CreatedUserRequest> _validator;

    public UserService(MiauDbContext db, IRequestValidator<CreatedUserRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <remarks>If the request contains invalid data or the CPF/e-mail are already registered, the operation fails.</remarks>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedUserResponse, ErrorResponse>>> CreateUserAsync(CreatedUserRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Check if request contains valid data
        if (!_validator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        // Check if CPF or e-mail have been registered by another user already
        if (await _db.Users.AnyAsync(x => x.Cpf == request.Cpf || x.Email == request.Email))
            return new BadRequestObjectResult(new ErrorResponse("CPF or e-mail have already been registered."));

        // Create the database user
        var dbUser = new UserEntity()
        {
            Cpf = request.Cpf,
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            Phone = request.Phone,
            HashedPassword = Encrypt.HashPassword(request.Password)
        };

        _db.Users.Add(dbUser);
        await _db.SaveChangesAsync();

        // TODO: handle authentication properly
        return new CreatedResult(location, new CreatedUserResponse(dbUser.Id, "placeholder_token"));
    }
}