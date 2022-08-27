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

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to appointments.
/// </summary>
public sealed class AppointmentService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<CreatedAppointmentRequest> _validator;
    private readonly IRequestValidator<UpdateAppointmentRequest> _validatorUpdate;

    public AppointmentService(MiauDbContext db, IRequestValidator<CreatedAppointmentRequest> validator, IRequestValidator<UpdateAppointmentRequest> validatorUpdate)
    {
        _db = db;
        _validator = validator;
        _validatorUpdate = validatorUpdate;
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

        var dbPet = await _db.Pets
            .AsTracking()
            .FirstOrDefaultAsyncEF(x => x.Id == request.PetId);

        if (dbPet is null)
            return new NotFoundObjectResult(new ErrorResponse($"No pet with Id {request.PetId} was found"));

        // Create the database appointment
        var dbAppointment = new AppointmentEntity()
        {
            Pet = dbPet,
            Price = request.Price,
            Type = request.Type,
            ScheduledTime = request.ScheduledTime
        };

        _db.Appointments.Add(dbAppointment);
        await _db.SaveChangesAsync();

        return new CreatedResult(location, new CreatedAppointmentResponse(dbAppointment.Id));
    }

    /// <summary>
    /// Returns a list of appointments.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<PagedResponse<AppointmentObject[]>, None>>> GetAppointmentsAsync(AppointmentParameters request)
    {
        var dbAppointments = await _db.Appointments
            .Include(x => x.Pet)
            .Include(x => x.Pet.User)
            .Where(x => x.Pet.User.Id == request.UserId && x.Pet.Id == request.PetId)
            .OrderBy(x => x.Id)
            .PageRange(request.PageNumber, request.PageSize)
            .Select(x => new AppointmentObject(x.Id, x.Pet.User.Id, x.Pet.Id, x.Price, x.Type, x.ScheduledTime))
            .ToArrayAsyncEF();

        if (dbAppointments.Length is 0)
            return new NotFoundResult();

        var remainingResultIds = await _db.Appointments
            .Where(x => !dbAppointments.Select(y => y.Id).Contains(x.Id) && x.Pet.User.Id == request.UserId && x.Pet.Id == request.PetId)
            .Select(x => x.Id)
            .ToArrayAsyncEF();

        var previousAmount = remainingResultIds.Count(x => x < dbAppointments[0].Id);
        var nextAmount = remainingResultIds.Count(x => x > dbAppointments[^1].Id);

        return new OkObjectResult(PagedResponse.Create(request.PageNumber, request.PageSize, previousAmount, nextAmount, dbAppointments.Length, dbAppointments));
    }

    /// <summary>
    /// Deletes the appointment with the given Id.
    /// </summary>
    /// <param name="appointmentId">The Id of the appointment to be deleted.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult> DeleteAppointmentAsync(int appointmentId)
    {
        return ((await _db.Appointments.DeleteAsync(p => p.Id == appointmentId)) is 0)
            ? new NotFoundResult()
            : new OkResult();
    }

    /// <summary>
    /// Updates an appointment.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<None, ErrorResponse>>> UpdateAppointmentByIdAsync(UpdateAppointmentRequest request)
    {
        // Check if request contains valid data
        if (!_validatorUpdate.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        var appointmentExists = await _db.Appointments
            .Include(x => x.Pet.User)
            .AnyAsyncEF(x => x.Id == request.Id && x.Pet.Id == request.PetId);

        if (!appointmentExists)
            return new NotFoundObjectResult(new ErrorResponse($"No appointment with the Id {request.Id} for the pet of Id {request.PetId} was found"));

        var dbAppointment = new AppointmentEntity()
        {
            Id = request.Id,
            Price = request.Price,
            Type = request.Type,
            ScheduledTime = request.ScheduledTime
        };

        _db.Appointments.Update(dbAppointment);
        await _db.SaveChangesAsync();

        return new OkResult();
    }
}
