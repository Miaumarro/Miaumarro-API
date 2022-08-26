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
        if (!Validate.IsPositive(request.Id, nameof(request.Id), out var idError)
            || !Validate.IsPositive(request.PetId, nameof(request.PetId), out idError))
        {
            errorMessages = errorMessages.Append(idError);
        }

        // Check Price
        if (!Validate.IsPositiveOrNeutral(request.Price, nameof(request.Price), out var priceError))
        {
            errorMessages = errorMessages.Append(priceError);
        }

        // Check ScheduledTime
        if (!Validate.IsFuture(request.ScheduledTime, nameof(request.ScheduledTime), out var scheduledTimeError))
        {
            errorMessages = errorMessages.Append(scheduledTimeError);
        }

        return !errorMessages.Any();
    }
}