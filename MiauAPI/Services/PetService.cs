using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Pagination;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using MiauAPI.Validators;
using LinqToDB;
using MiauDatabase.Enums;
using MiauAPI.Enums;
using MiauAPI.Models.QueryObjects;


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

        // Checks the UserId
        var dbUser = await _db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (dbUser == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No User with the Id = {request.UserId} was found"));
        }

        string? imagePath = null;

        //Create the path for the Pet Image
        if (request.ImagePath != null)
        {
            var path = $"Data/{request.UserId}/pets";
            if ((!Directory.Exists(path)))
            {
                Directory.CreateDirectory(path);
            }
            var filename = request.ImagePath.FileName!;
            using var fileStream = new FileStream(Path.Combine(path, filename), FileMode.Create);
            await request.ImagePath.CopyToAsync(fileStream);
            imagePath = $"Data/{request.UserId}/pets" + filename;
        }

        // Create the database pet

        var dbPet = new PetEntity()
        {
            User = dbUser,
            Name = request.Name,
            Type = request.Type,
            Gender = request.Gender,
            Breed = request.Breed,
            ImagePath = imagePath,
            DateOfBirth = request.DateOfBirth
        };

        _db.Pets.Update(dbPet);
        await _db.SaveChangesAsync();

        // TODO: handle authentication properly
        return new CreatedResult(location, new CreatedPetResponse(dbPet.Id));
    }

}
