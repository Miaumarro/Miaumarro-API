using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using LinqToDB;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Extensions;
using Microsoft.EntityFrameworkCore;
using LinqToDB.EntityFrameworkCore;
using OneOf.Types;
using Kotz.Extensions;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to pet.
/// </summary>
public sealed class PetService
{
    private const string _imageDirName = "pets";
    private readonly MiauDbContext _db;
    private readonly FileService _fileService;
    private readonly IRequestValidator<CreatedPetRequest> _validator;
    private readonly IRequestValidator<UpdatePetRequest> _validatorUpdate;

    // TODO: perhaps only the path to the image should be returned, instead of its binary data
    public PetService(MiauDbContext db, FileService fileService, IRequestValidator<CreatedPetRequest> validator, IRequestValidator<UpdatePetRequest> validatorUpdate)
    {
        _db = db;
        _fileService = fileService;
        _validator = validator;
        _validatorUpdate = validatorUpdate;
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

        var dbUser = await _db.Users
            .AsTracking()
            .FirstOrDefaultAsyncEF(x => x.Id == request.UserId);

        if (dbUser is null)
            return new NotFoundObjectResult(new ErrorResponse($"No User with the Id = {request.UserId} was found"));

        var imagePath = (request.Image is not null)
            ? await _fileService.SaveFileAsync(request.Image, Path.Combine(_imageDirName, request.UserId.ToString()), request.Name)
            : null;

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

        _db.Pets.Add(dbPet);
        await _db.SaveChangesAsync();

        return new CreatedResult(location, new CreatedPetResponse(dbPet.Id));
    }

    /// <summary>
    /// Returns a list of pets.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<PagedResponse<PetObject[]>, None>>> GetPetsAsync(PetParameters request)
    {
        var dbImagePathsAndPets = await _db.Pets
            .Include(x => x.User)
            .Where(x => x.User.Id == request.UserId)
            .OrderBy(x => x.Id)
            .PageRange(request.PageNumber, request.PageSize)
            .Select(x => new
            {
                x.ImagePath,
                Pet = new PetObject(x.Id, x.User.Id, x.Name, x.Type, x.Gender, x.DateOfBirth, x.Breed, null)
            })
            .ToArrayAsyncEF();

        if (dbImagePathsAndPets.Length is 0)
            return new NotFoundResult();

        var petResponses = await dbImagePathsAndPets
            .Where(x => !string.IsNullOrWhiteSpace(x.ImagePath))
            .Select(async x => x.Pet with { Image = await _fileService.ReadFileAsync(x.ImagePath!) })
            .WhenAllAsync();

        var remainingResultIds = await _db.Pets
            .Include(x => x.User)
            .Where(x => !petResponses.Select(y => y.Id).Contains(x.Id) && x.User.Id == request.UserId)
            .Select(x => x.Id)
            .ToArrayAsyncEF();

        var previousAmount = remainingResultIds.Count(x => x < petResponses[0].Id);
        var nextAmount = remainingResultIds.Count(x => x > petResponses[^1].Id);

        return new OkObjectResult(PagedResponse.Create(request.PageNumber, request.PageSize, previousAmount, nextAmount, petResponses.Length, petResponses));
    }

    /// <summary>
    /// Return the pet with the given Id.
    /// </summary>
    /// <param name="petId">The Id of the pet to be searched.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<PetObject, None>>> GetPetByIdAsync(int petId)
    {
        var dbImagePathAndPet = await _db.Pets
            .Include(x => x.User)
            .Where(x => x.Id == petId)
            .Select(x => new
            {
                x.ImagePath,
                Pet = new PetObject(x.Id, x.User.Id, x.Name, x.Type, x.Gender, x.DateOfBirth, x.Breed, null)
            })
            .FirstOrDefaultAsyncEF();

        if (dbImagePathAndPet is null)
            return new NotFoundResult();

        var result = (string.IsNullOrWhiteSpace(dbImagePathAndPet.ImagePath))
            ? dbImagePathAndPet.Pet
            : dbImagePathAndPet.Pet with { Image = await _fileService.ReadFileAsync(dbImagePathAndPet.ImagePath) };
        
        return new OkObjectResult(result);
    }

    /// <summary>
    /// Deletes the pet with the given Id.
    /// </summary>
    /// <param name="petId">The Id of the pet to be deleted.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult> DeletePetByIdAsync(int petId)
    {
        return ((await _db.Pets.DeleteAsync(x => x.Id == petId)) is 0)
            ? new NotFoundResult()
            : new OkResult();
    }

    /// <summary>
    /// Updates a pet.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<None, ErrorResponse>>> UpdatePetAsync(UpdatePetRequest request)
    {
        // Check if request contains valid data
        if (!_validatorUpdate.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        var currentDbPet = await _db.Pets
            .Include(x => x.User)
            .FirstOrDefaultAsyncEF(x => x.User.Id == request.UserId && x.Id == request.Id);

        if (currentDbPet is null)
            return new NotFoundResult();

        var subDirectoryName = Path.Combine(_imageDirName, request.UserId.ToString());

        // If new image doesn't exist, delete the old image
        if (request.Image is null && _fileService.FileExists(subDirectoryName, request.Name))
            _fileService.DeleteFile(subDirectoryName, request.Name);

        // If new image exists and old image with a different name also exists, delete the old image
        if (request.Image is not null
            && currentDbPet.ImagePath is not null
            && !currentDbPet.Name.Equals(request.Name, StringComparison.Ordinal))
        {
            File.Delete(currentDbPet.ImagePath);
        }

        var imagePath = (request.Image is not null)
            ? await _fileService.SaveFileAsync(request.Image, subDirectoryName, request.Name)
            : null;

        await _db.Pets
            .Where(x => x.Id == request.Id)
            .UpdateAsync(x => new PetEntity()
            {
                Name = request.Name,
                Type = request.Type,
                Gender = request.Gender,
                Breed = request.Breed,
                ImagePath = (x.ImagePath != imagePath) ? imagePath : x.ImagePath,
                DateOfBirth = request.DateOfBirth,
            });

        return new OkResult();
    }
}
