using MiauAPI.Models.QueryObjects;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Pagination;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to address.
/// </summary>
public sealed class AddressService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<CreatedAddressRequest> _validator;

    public AddressService(MiauDbContext db, IRequestValidator<CreatedAddressRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    /// <summary>
    /// Creates a new address.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedAddressResponse, ErrorResponse>>> CreateAddressAsync(CreatedAddressRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Check if request contains valid data
        if (!_validator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        // Checks the UserId
        if (request.UserId == 0)
            return new BadRequestObjectResult(new ErrorResponse($"The address must be related to a user. 'UserId = {request.UserId}'"));
        var dbUser = await _db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (dbUser == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No User with the Id = {request.UserId} was found"));
        }

        // Create the database address
        var dbAddress = new AddressEntity()
        {
            User = dbUser,
            Address = request.Address,
            Number = request.Number,
            Reference = request.Reference,
            Complement = request.Complement,
            Neighborhood = request.Neighborhood,
            City = request.City,
            State = request.State,
            Destinatary = request.Destinatary,
            Cep = request.Cep
        };

        _db.Addresses.Update(dbAddress);
        await _db.SaveChangesAsync();

        // TODO: handle authentication properly
        return new CreatedResult(location, new CreatedAddressResponse(dbAddress.Id));
    }

    /// <summary>
    /// Returns a list of addresses.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetAddressResponse, ErrorResponse>>> GetAddressAsync(AddressParameters addressParameters)
    {

        var dbAddresses = _db.Addresses.Select(p => new AddressObject
        {
            UserId = p.User.Id,
            Id = p.Id,
            Address = p.Address,
            Number = p.Number,
            Reference = p.Reference,
            Complement = p.Complement,
            Neighborhood = p.Neighborhood,
            City = p.City,
            State = p.State,
            Destinatary = p.Destinatary,
            Cep = p.Cep
        });

        if (addressParameters.UserId != 0)
        {
            dbAddresses = dbAddresses.Where(p => p.UserId == addressParameters.UserId);
        }

        var dbAddressesList = await dbAddresses.ToListAsync();

        if (dbAddressesList.Count == 0)
        {
            return new NotFoundObjectResult("No addresses with the given paramenters were found.");
        }

        var dbAddressesPaged = PagedList<AddressObject>.ToPagedList(
                        dbAddressesList,
                        addressParameters.PageNumber,
                        addressParameters.PageSize);

        return new OkObjectResult(new GetAddressResponse(dbAddressesPaged));
    }

    /// <summary>
    /// Return the address with the given Id.
    /// </summary>
    /// <param name="addressId">The Id of the address to be searched.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetAddressByIdResponse, ErrorResponse>>> GetAddressByIdAsync(int addressId)
    {

        var dbAddress = await _db.Addresses.Where(p => p.Id == addressId)
                                            .Select(p => new AddressObject
                                            {
                                                Id = p.Id,
                                                UserId = p.User.Id,
                                                Address = p.Address,
                                                Number = p.Number,
                                                Reference = p.Reference,
                                                Complement = p.Complement,
                                                Neighborhood = p.Neighborhood,
                                                City = p.City,
                                                State = p.State,
                                                Destinatary = p.Destinatary,
                                                Cep = p.Cep
                                            })
                                            .FirstOrDefaultAsync();

        return dbAddress == null
                ? new NotFoundObjectResult(new ErrorResponse($"No address with the Id = {addressId} was found"))
                : new OkObjectResult(new GetAddressByIdResponse(dbAddress));
    }

}
