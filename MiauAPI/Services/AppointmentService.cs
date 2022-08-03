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
/// Handles requests pertaining to appointments.
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

        _db.Appointments.Update(dbAppointment);
        await _db.SaveChangesAsync();

        // TODO: handle authentication properly
        return new CreatedResult(location, new CreatedAppointmentResponse(dbAppointment.Id));
    }

    /// <summary>
    /// Returns a list of appointments.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetAppointmentResponse, ErrorResponse>>> GetAppointmentAsync(AppointmentParameters appointmentParameters)
    {

        var dbAppointments = _db.Appointments.Select(p => new AppointmentObject
        {
            Id = p.Id,
            UserId = p.Pet.User.Id,
            PetId = p.Pet.Id,
            Price = p.Price,
            Type = p.Type,
            ScheduledTime = p.ScheduledTime
        });

        if (appointmentParameters.UserId != 0)
        {
            dbAppointments = dbAppointments.Where(p => p.UserId == appointmentParameters.UserId);
        }

        if (appointmentParameters.PetId != 0)
        {
            dbAppointments = dbAppointments.Where(p => p.PetId == appointmentParameters.PetId);
        }

        var dbAppointmentsList = await dbAppointments.ToListAsync();

        if (dbAppointmentsList.Count == 0)
        {
            return new NotFoundObjectResult("No appointments with the given paramenters were found.");
        }

        var dbAppointmentsPaged = PagedList<AppointmentObject>.ToPagedList(
                        dbAppointmentsList,
                        appointmentParameters.PageNumber,
                        appointmentParameters.PageSize);

        return new OkObjectResult(new GetAppointmentResponse(dbAppointmentsPaged));
    }

    /// <summary>
    /// Return the appointment with the given Id.
    /// </summary>
    /// <param name="appointmentId">The Id of the appointment to be searched.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetAppointmentByIdResponse, ErrorResponse>>> GetAppointmentByIdAsync(int appointmentId)
    {

        var dbAppointment = await _db.Appointments.Where(p => p.Id == appointmentId)
                                            .Select(p => new AppointmentObject
                                            {
                                                Id = p.Id,
                                                UserId = p.Pet.User.Id,
                                                PetId = p.Pet.Id,
                                                Price = p.Price,
                                                Type = p.Type,
                                                ScheduledTime = p.ScheduledTime
                                            })
                                            .FirstOrDefaultAsync();

        return dbAppointment == null
                ? new NotFoundObjectResult(new ErrorResponse($"No appointment with the Id = {appointmentId} was found"))
                : new OkObjectResult(new GetAppointmentByIdResponse(dbAppointment));
    }
}
