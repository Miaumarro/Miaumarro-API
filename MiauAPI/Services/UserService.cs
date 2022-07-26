using MiauAPI.Common;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services.Abstractions;
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
public sealed class UserService : IRequestValidator<CreatedUserRequest>
{
    private readonly MiauDbContext _db;

    public UserService(MiauDbContext db)
        => _db = db;

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
        // Check if request contains valid data
        if (!IsRequestValid(request, out var errorMessages))
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

    public bool IsRequestValid(CreatedUserRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        // Check CPF
        if (Validator.IsNull(request.Cpf, nameof(request.Cpf), out var cpfError)
            || !Validator.IsTextInRange(request.Cpf, 11, 11, nameof(request.Cpf), out cpfError)
            || !Validator.HasOnlyDigits(request.Cpf, nameof(request.Cpf), out cpfError))
        {
            errorMessages = errorMessages.Append(cpfError);
        }

        // Check name
        if (Validator.IsNull(request.Name, nameof(request.Name), out var nameError)
            || !Validator.IsTextInRange(request.Name, 30, nameof(request.Name), out nameError))
        {
            errorMessages = errorMessages.Append(nameError);
        }

        // Check surname
        if (Validator.IsNull(request.Surname, nameof(request.Surname), out var surnameError)
            || !Validator.IsTextInRange(request.Surname, 60, nameof(request.Surname), out surnameError))
        {
            errorMessages = errorMessages.Append(surnameError);
        }

        // Check e-mail
        if (Validator.IsNull(request.Email, nameof(request.Email), out var emailError)
            || !Validator.IsTextInRange(request.Email, 60, nameof(request.Email), out emailError)
            || !Validator.IsEmail(request.Email, out emailError))
        {
            errorMessages = errorMessages.Append(emailError);
        }

        // Check phone
        if (request.Phone is not null
            && (!Validator.IsTextInRange(request.Phone, 10, 14, nameof(request.Phone), out var phoneError)
            || !Validator.HasOnlyDigits(request.Phone, nameof(request.Phone), out phoneError)))
        {
            errorMessages = errorMessages.Append(phoneError);
        }

        // Check password
        if (Validator.IsNull(request.Password, nameof(request.Password), out var passwordError)
            || !Validator.IsTextInRange(request.Password, 100, nameof(request.Password), out passwordError))
        {
            errorMessages = errorMessages.Append(passwordError);
        }

        return !errorMessages.Any();
    }
}