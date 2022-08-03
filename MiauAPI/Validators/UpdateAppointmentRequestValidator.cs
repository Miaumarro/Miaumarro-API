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

        // Check Price
        if (!Validate.IsPositive(request.Price, nameof(request.Price), out var priceError))
        {
            errorMessages = errorMessages.Append(priceError);
        }

        return !errorMessages.Any();

    }
}