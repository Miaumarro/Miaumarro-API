using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
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

        // Create the database pet
        var dbAddress = new AddressEntity()
        {
            User = _db.Users.First(x => x.Id == request.UserId),
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

        _db.Addresses.Add(dbAddress);
        await _db.SaveChangesAsync();

        // TODO: handle authentication properly
        return new CreatedResult(location, new CreatedPetResponse(dbAddress.Id));
    }
}
