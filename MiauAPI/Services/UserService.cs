using LinqToDB;
using MiauAPI.Extensions;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using Encrypt = BCrypt.Net.BCrypt;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to users.
/// </summary>
public sealed class UserService
{
    private readonly MiauDbContext _db;
    private readonly AuthenticationService _loginService;
    private readonly IRequestValidator<CreatedUserRequest> _validator;
    private readonly IRequestValidator<UserParameters> _userParamValidator;
    private readonly IRequestValidator<UpdateUserRequest> _validatorUpdate;
    private readonly IRequestValidator<UpdateUserPasswordRequest> _validatorUpdatePassword;

    public UserService(MiauDbContext db, AuthenticationService loginService, IRequestValidator<CreatedUserRequest> validator, IRequestValidator<UserParameters> userParamValidator, IRequestValidator<UpdateUserRequest> validatorUpdate, IRequestValidator<UpdateUserPasswordRequest> validatorUpdatePassword)
    {
        _db = db;
        _loginService = loginService;
        _validator = validator;
        _userParamValidator = userParamValidator;
        _validatorUpdate = validatorUpdate;
        _validatorUpdatePassword = validatorUpdatePassword;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <remarks>If the request contains invalid data or the CPF/e-mail are already registered, the operation fails.</remarks>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<UserAuthenticationResponse, ErrorResponse>>> RegisterUserAsync(CreatedUserRequest request, string location)
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

        var expireAt = _loginService.TokenExpirationTime;

        return new CreatedResult(location, new UserAuthenticationResponse(dbUser.Id, _loginService.GenerateSessionToken(dbUser, expireAt), expireAt));
    }

    /// <summary>
    /// Returns a collection of users that meet the specified criteria.
    /// </summary>
    /// <param name="request">The criteria to search for.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<PagedResponse<UserObject[]>, ErrorResponse>>> GetUsersAsync(UserParameters request)
    {
        if (!_userParamValidator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        var dbUsers = await _db.Users
            .Where(x => request.Ids.Contains(x.Id) || request.Cpfs.Contains(x.Cpf) || request.Emails.Contains(x.Email))
            .OrderBy(x => x.Id)
            .Select(x => new UserObject(x.Id, x.Cpf, x.Name, x.Surname, x.Email, x.Phone))
            .PageRange(request.PageNumber, request.PageSize)
            .ToArrayAsync();

        if (dbUsers.Length is 0)
            return new NotFoundObjectResult(new ErrorResponse($"No users with the given parameters were found."));

        var remainingResultIds = await _db.Users
            .Where(x => !dbUsers.Select(y => y.Id).Contains(x.Id) && (request.Ids.Contains(x.Id) || request.Cpfs.Contains(x.Cpf) || request.Emails.Contains(x.Email)))
            .Select(x => x.Id)
            .ToArrayAsync();

        var previousAmount = remainingResultIds.Count(x => x < dbUsers[0].Id);
        var nextAmount = remainingResultIds.Count(x => x > dbUsers[^1].Id);

        return new OkObjectResult(PagedResponse.Create(request.PageNumber, request.PageSize, previousAmount, nextAmount, dbUsers.Length, dbUsers));
    }

    /// <summary>
    /// Returns the user with the specified Id.
    /// </summary>
    /// <param name="userId">The Id of the user.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<UserObject, None>>> GetUserByIdAsync(int userId)
    {
        var dbUser = await _db.Users
            .Where(x => x.Id == userId)
            .Select(x => new UserObject(x.Id, x.Cpf, x.Name, x.Surname, x.Email, x.Phone))
            .FirstOrDefaultAsync();

        return (dbUser is null)
            ? new NotFoundResult()
            : new OkObjectResult(dbUser);
    }

    /// <summary>
    /// Deletes users that meet the specified criteria.
    /// </summary>
    /// <param name="id">The criteria to search for.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeleteUsersAsync(UserParameters userParameters)
    {
        if (!_userParamValidator.IsRequestValid(userParameters, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        var result = await _db.Users.DeleteAsync(x => userParameters.Ids.Contains(x.Id) || userParameters.Cpfs.Contains(x.Cpf) || userParameters.Emails.Contains(x.Email));

        return (result is 0)
            ? new NotFoundObjectResult(new ErrorResponse($"No user with the specified criteria was found."))
            : new OkObjectResult(new DeleteResponse($"Successfully deleted {result} users."));
    }

    /// <summary>
    /// Updates an user.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<None, ErrorResponse>>> UpdateUserAsync(UpdateUserRequest request)
    {
        // Check if request contains valid data
        if (!_validatorUpdate.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        var result = await _db.Users
            .Where(x => x.Id == request.Id)
            .UpdateAsync(x => new UserEntity()
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                Phone = request.Phone
            });

        return (result is 0)
            ? new NotFoundResult()
            : new OkResult();
    }

    /// <summary>
    /// Updates a user's password.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse>>> UpdateUserPasswordAsync(UpdateUserPasswordRequest request)
    {
        // Check if request contains valid data
        if (!_validatorUpdatePassword.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        // Get user and check if their old password match with the current one
        // If it doesn't, return a failed request
        var userHashedPassword = await _db.Users
            .Where(x => x.Id == request.Id)
            .Select(x => x.HashedPassword)
            .FirstOrDefaultAsync();

        if (userHashedPassword is null)
            return new NotFoundObjectResult(new ErrorResponse($"No user with Id {request.Id} was found."));

        if (!Encrypt.Verify(request.OldPassword, userHashedPassword))
            return new BadRequestObjectResult(new ErrorResponse("Old password does not match the current password."));

        // Update the user's password
        await _db.Users
            .Where(x => x.Id == request.Id)
            .UpdateAsync(x => new UserEntity() { HashedPassword = Encrypt.HashPassword(request.NewPassword) });

        // TODO: expire the current session token and return a new session token here
        return new OkResult();
    }
}