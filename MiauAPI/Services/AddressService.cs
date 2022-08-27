using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using MiauAPI.Extensions;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to address.
/// </summary>
public sealed class AddressService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<CreatedAddressRequest> _createValidator;
    private readonly IRequestValidator<UpdateAddressRequest> _updateValidator;

    public AddressService(MiauDbContext db, IRequestValidator<CreatedAddressRequest> validator, IRequestValidator<UpdateAddressRequest> validatorUpdate)
    {
        _db = db;
        _createValidator = validator;
        _updateValidator = validatorUpdate;
    }

    /// <summary>
    /// Creates a new address.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedAddressResponse, ErrorResponse, None>>> CreateAddressAsync(CreatedAddressRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Check if request contains valid data
        if (!_createValidator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        var dbUser = await _db.Users
            .AsTracking()
            .FirstOrDefaultAsyncEF(x => x.Id == request.UserId);

        if (dbUser is null)
            return new NotFoundResult();

        // Create the database address
        var dbAddress = new AddressEntity()
        {
            User = dbUser,
            Address = request.Address,
            Number = request.AddressNumber,
            Reference = request.Reference,
            Complement = request.Complement,
            Neighborhood = request.Neighborhood,
            City = request.City,
            State = request.State,
            Destinatary = request.Destinatary,
            Cep = request.Cep
        };

        _db.Addresses.Add(dbAddress);
        await _db.SaveChangesAsync();

        return new CreatedResult(location, new CreatedAddressResponse(dbAddress.Id));
    }

    /// <summary>
    /// Returns a list of addresses.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<PagedResponse<AddressObject[]>, None>>> GetAddressesAsync(AddressParameters request)
    {
        var dbAddresses = await _db.Addresses
            .Where(x => x.User.Id == request.UserId)
            .OrderBy(x => x.Id)
            .PageRange(request.PageNumber, request.PageSize)
            .Select(x => new AddressObject(
                    x.Id,
                    x.User.Id,
                    x.State,
                    x.City,
                    x.Neighborhood,
                    x.Cep,
                    x.Address,
                    x.Number,
                    x.Complement,
                    x.Reference,
                    x.Destinatary
                )
            )
            .ToArrayAsyncEF();

        if (dbAddresses.Length is 0)
            return new NotFoundResult();

        var remainingResultIds = await _db.Addresses
            .Where(x => !dbAddresses.Select(y => y.Id).Contains(x.Id) && x.User.Id == request.UserId)
            .Select(x => x.Id)
            .ToArrayAsyncEF();

        var previousAmount = remainingResultIds.Count(x => x < dbAddresses[0].Id);
        var nextAmount = remainingResultIds.Count(x => x > dbAddresses[^1].Id);

        return new OkObjectResult(PagedResponse.Create(request.PageNumber, request.PageSize, previousAmount, nextAmount, dbAddresses.Length, dbAddresses));
    }

    /// <summary>
    /// Deletes the address with the given Id.
    /// </summary>
    /// <param name="addressId">The Id of the address to be deleted.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult> DeleteAddressAsync(int addressId)
    {
        return ((await _db.Addresses.DeleteAsync(p => p.Id == addressId)) is 0)
            ? new NotFoundResult()
            : new OkResult();
    }

    /// <summary>
    /// Updates an address.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse, None>>> UpdateAddressAsync(UpdateAddressRequest request)
    {
        // Check if request contains valid data
        if (!_updateValidator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        var addressExists = await _db.Addresses
            .Include(x => x.User)
            .AnyAsyncEF(x => x.Id == request.Id && x.User.Id == request.UserId);

        if (!addressExists)
            return new NotFoundResult();

        await _db.Addresses
            .Where(x => x.Id == request.Id)
            .UpdateAsync(x => new AddressEntity()
            {
                Id = request.Id,
                Address = request.Address,
                Number = request.AddressNumber,
                Reference = request.Reference,
                Complement = request.Complement,
                Neighborhood = request.Neighborhood,
                City = request.City,
                State = request.State,
                Destinatary = request.Destinatary,
                Cep = request.Cep
            });

        return new OkResult();
    }
}
