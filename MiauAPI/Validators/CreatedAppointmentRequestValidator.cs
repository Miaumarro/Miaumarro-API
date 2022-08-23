using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="AppointmentRequest"/>.
/// </summary>
public sealed class CreatedAppointmentRequestValidator : IRequestValidator<CreatedAppointmentRequest>
{
    public bool IsRequestValid(CreatedAppointmentRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        // Check id
        if (request.PetId <= 0)
        {
            errorMessages = errorMessages.Append($"{nameof(request.PetId)} must be valid.");
        }

        // Check Price
        if (!Validate.IsPositive(request.Price, nameof(request.Price), out var priceError))
        {
            errorMessages = errorMessages.Append(priceError);
        }

        // Check ScheduledTime
        if (!Validate.IsFutureDate(request.ScheduledTime, nameof(request.ScheduledTime), out var scheduledTimeError))
        {
            errorMessages = errorMessages.Append(scheduledTimeError);
        }

        return !errorMessages.Any();
    }
}