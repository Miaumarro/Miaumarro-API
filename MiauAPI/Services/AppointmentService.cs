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
/// Handles requests pertaining to products.
/// </summary>
public sealed class AppointmentService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<CreatedAppointmentRequest> _validator;

    public AppointmentService(MiauDbContext db, IRequestValidator<CreatedAppointmentRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    /// <summary>
    /// Creates a new appointment.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedAppointmentResponse, ErrorResponse>>> CreateAppointmentAsync(CreatedAppointmentRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Check if request contains valid data
        if (!_validator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        // Checks the PetId
        var dbPet = await _db.Pets.FirstOrDefaultAsync(x => x.Id == request.PetId);
        if (dbPet == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No pet with the Id = {request.PetId} was found"));
        }

        // Create the database appointment
        var dbAppointment = new AppointmentEntity()
        {
            Pet = dbPet,
            Price = request.Price,
            Type = request.Type,
            ScheduledTime = request.ScheduledTime
        };

        await _db.Appointments.AddAsync(dbAppointment);
        await _db.SaveChangesAsync();

        // TODO: handle authentication properly
        return new CreatedResult(location, new CreatedAppointmentResponse(dbAppointment.Id));
    }
}
