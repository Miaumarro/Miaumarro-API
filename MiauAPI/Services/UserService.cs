using LinqToDB;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Pagination;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IRequestValidator<UpdateUserRequest> _validatorUpdate;

    public UserService(MiauDbContext db, IRequestValidator<CreatedUserRequest> validator, IRequestValidator<UpdateUserRequest> validatorUpdate)
    {
        _db = db;
        _validator = validator;
        _validatorUpdate = validatorUpdate;
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

    /// <summary>
    /// Returns a list of users.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetUserResponse, ErrorResponse>>> GetUserAsync(UserParameters userParameters)
    {

        var dbUsers = _db.Users.Select(p => new UserObject
        {
            Id = p.Id,
            Cpf = p.Cpf,
            Name = p.Name,
            Surname = p.Surname,
            Email = p.Email,
            Phone = p.Phone,
            Password = p.HashedPassword
        });

        if (userParameters.Cpf != null)
        {
            dbUsers = dbUsers.Where(p => p.Cpf.Contains(userParameters.Cpf));
        }

        var dbUsersList = await dbUsers.ToListAsync();

        return (dbUsersList.Count is 0)
            ? new NotFoundObjectResult(new ErrorResponse($"No users with the given parameters were found."))
            : new OkObjectResult(new GetUserResponse(PagedList<UserObject>.ToPagedList(
                                                        dbUsersList,
                                                        userParameters.PageNumber,
                                                        userParameters.PageSize)));
    }

    /// <summary>
    /// Return the user with the given Id.
    /// </summary>
    /// <param name="id">The id of the user to be searched.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetUserByIdResponse, ErrorResponse>>> GetUserByIdAsync(int id)
    {

        var dbUser = await _db.Users.Where(p => p.Id == id)
                                            .Select(p => new UserObject
                                            {
                                                Id = p.Id,
                                                Cpf = p.Cpf,
                                                Name = p.Name,
                                                Surname = p.Surname,
                                                Email = p.Email,
                                                Phone = p.Phone,
                                                Password = p.HashedPassword
                                            })
                                            .FirstOrDefaultAsync();

        return dbUser == null
                ? new NotFoundObjectResult(new ErrorResponse($"No user with the Id = {id} was found"))
                : new OkObjectResult(new GetUserByIdResponse(dbUser));
    }

    /// <summary>
    /// Deletes the user with the given Id.
    /// </summary>
    /// <param name="id">The Id of the user to be deleted.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeleteUserByIdAsync(int id)
    {
        return ((await _db.Users.DeleteAsync(p => p.Id == id)) is 0)
            ? new NotFoundObjectResult(new ErrorResponse($"No user with the Id = {id} was found"))
            : new OkObjectResult(new DeleteResponse($"Successful delete user with the Id = {id}"));
    }

    /// <summary>
    /// Updates an user.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse>>> UpdateUserByIdAsync(UpdateUserRequest request)
    {
        // Check if request contains valid data
        if (!_validatorUpdate.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        var dbUser = await _db.Users.FindAsync(request.Id);

        if (dbUser == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No user with the Id = {request.Id} was found"));
        }

        dbUser = new UserEntity()
        {
            Id = request.Id,
            Cpf = dbUser.Cpf,
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            Phone = request.Phone,
            HashedPassword = Encrypt.HashPassword(request.Password)
        };

        _db.Users.Update(dbUser);

        await _db.SaveChangesAsync();

        return new OkObjectResult(new UpdateResponse($"Successful update user with the Id = {request.Id}"));

    }
}