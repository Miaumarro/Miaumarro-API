using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="UpdateAppointmentRequest"/>.
/// </summary>
public sealed class UpdateAppointmentRequestValidator : IRequestValidator<UpdateAppointmentRequest>
{
    public bool IsRequestValid(UpdateAppointmentRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        // Check ids
        if (request.Id <= 0 || request.PetId <= 0)
        {
            errorMessages = errorMessages.Append("Ids must be higher than zero.");
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