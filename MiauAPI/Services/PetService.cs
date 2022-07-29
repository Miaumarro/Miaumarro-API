using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
//using Encrypt = BCrypt.Net.BCrypt;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to pet.
/// </summary>
public sealed class PetService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<CreatedPetRequest> _validator;

    public PetService(MiauDbContext db, IRequestValidator<CreatedPetRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    /// <summary>
    /// Creates a new pet.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedPetResponse, ErrorResponse>>> CreatePetAsync(CreatedPetRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Check if request contains valid data
        if (!_validator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        //Create the path for the Pet Image
        var path = $"Data/{request.UserId}/pets";
        if ((!Directory.Exists(path)))
        {
            Directory.CreateDirectory(path);
        }
        var filename = request.ImagePath.FileName;
        using (var fileStream = new FileStream(Path.Combine(path, filename), FileMode.Create))
        {
            await request.ImagePath.CopyToAsync(fileStream);
        }

        // Create the database pet
        var dbPet = new PetEntity()
        {
            User = _db.Users.First(x => x.Id == request.UserId),
            Name = request.Name,
            Type = request.Type,
            Gender = request.Gender,
            Breed = request.Breed,
            ImagePath = "images/" + filename,
            DateOfBirth = request.DateOfBirth,
        };

        _db.Pets.Add(dbPet);
        await _db.SaveChangesAsync();

        // TODO: handle authentication properly
        return new CreatedResult(location, new CreatedPetResponse(dbPet.Id));
    }
}
