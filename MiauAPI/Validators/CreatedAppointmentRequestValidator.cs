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

        // Check Price
        if (!Validate.IsPositive(request.Price, nameof(request.Price), out var priceError))
        {
            errorMessages = errorMessages.Append(priceError);
        }

        return !errorMessages.Any();

    }
}