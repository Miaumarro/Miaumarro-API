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
    private readonly IRequestValidator<UpdatePetRequest> _validatorUpdate;

    public PetService(MiauDbContext db, IRequestValidator<CreatedPetRequest> validator, IRequestValidator<UpdatePetRequest> validatorUpdate)
    {
        _db = db;
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

        // Checks the UserId
        if(request.UserId == 0)
            return new BadRequestObjectResult(new ErrorResponse($"The pet must be related to a user. 'UserId = {request.UserId}'"));
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

    /// <summary>
    /// Returns a list of pets.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetPetResponse, ErrorResponse>>> GetPetAsync(PetParameters petParameters)
    {

        var dbPets = _db.Pets.Select(p => new PetObject
        {
            UserId = p.User.Id,
            Id = p.Id,
            Name = p.Name,
            Type = p.Type,
            Gender = p.Gender,
            Breed = p.Breed!,
            ImagePath = p.ImagePath!,
            DateOfBirth = p.DateOfBirth
        });

        if (petParameters.UserId != 0)
        {
            dbPets = dbPets.Where(p => p.UserId == petParameters.UserId);
        }

        var dbPetsList = await dbPets.ToListAsync();

        if (dbPetsList.Count == 0)
        {
            return new NotFoundObjectResult("No pets with the given paramenters were found.");
        }

        var dbPetsPaged = PagedList<PetObject>.ToPagedList(
                        dbPetsList,
                        petParameters.PageNumber,
                        petParameters.PageSize);

        return new OkObjectResult(new GetPetResponse(dbPetsPaged));
    }

    /// <summary>
    /// Return the pet with the given Id.
    /// </summary>
    /// <param name="petId">The Id of the pet to be searched.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetPetByIdResponse, ErrorResponse>>> GetPetByIdAsync(int petId)
    {

        var dbPet = await _db.Pets.Where(p => p.Id == petId)
                                            .Select(p => new PetObject
                                            {
                                                Id = p.Id,
                                                UserId = p.User.Id,
                                                Name = p.Name,
                                                Type = p.Type,
                                                Gender = p.Gender,
                                                Breed = p.Breed!,
                                                ImagePath = p.ImagePath!,
                                                DateOfBirth = p.DateOfBirth
                                            })
                                            .FirstOrDefaultAsync();

        return dbPet == null
                ? new NotFoundObjectResult(new ErrorResponse($"No pet with the Id = {petId} was found"))
                : new OkObjectResult(new GetPetByIdResponse(dbPet));
    }

    /// <summary>
    /// Deletes the pet with the given Id.
    /// </summary>
    /// <param name="petId">The Id of the pet to be deleted.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeletePetByIdAsync(int petId)
    {

        var dbPet = await _db.Pets.FindAsync(petId);
        if (dbPet == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No pet with the Id = {petId} was found"));
        }

        await _db.Pets.DeleteAsync(p => p.Id == petId);
        await _db.SaveChangesAsync();

        return new OkObjectResult(new DeleteResponse($"Successfull delete pet with the Id = {petId}"));
    }

    /// <summary>
    /// Updates a pet.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse>>> UpdatePetByIdAsync(UpdatePetRequest request)
    {
        // Check if request contains valid data
        if (!_validatorUpdate.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        var dbPet = await _db.Pets.FindAsync(request.Id);

        if (dbPet == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No pet with the Id = {request.Id} was found"));
        }

        // Checks the UserId
        if (request.UserId == 0)
            return new BadRequestObjectResult(new ErrorResponse($"The pet must be related to a user. 'UserId = {request.UserId}'"));
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

        dbPet = new PetEntity()
        {
            Id = request.Id,
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

        return new OkObjectResult(new UpdateResponse($"Successfull update pet with the Id = {request.Id}"));

    }

}
