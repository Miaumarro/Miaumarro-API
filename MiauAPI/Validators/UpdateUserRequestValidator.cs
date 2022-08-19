using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="UpdateUserRequest"/>.
/// </summary>
public sealed class UpdateUserRequestValidator : IRequestValidator<UpdateUserRequest>
{
    public bool IsRequestValid(UpdateUserRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        // Check name
        if (Validate.IsNullOrWhiteSpace(request.Name, nameof(request.Name), out var nameError)
            || !Validate.IsTextInRange(request.Name, 30, nameof(request.Name), out nameError))
        {
            errorMessages = errorMessages.Append(nameError);
        }

        // Check surname
        if (Validate.IsNullOrWhiteSpace(request.Surname, nameof(request.Surname), out var surnameError)
            || !Validate.IsTextInRange(request.Surname, 60, nameof(request.Surname), out surnameError))
        {
            errorMessages = errorMessages.Append(surnameError);
        }

        // Check e-mail
        if (Validate.IsNullOrWhiteSpace(request.Email, nameof(request.Email), out var emailError)
            || !Validate.IsTextInRange(request.Email, 60, nameof(request.Email), out emailError)
            || !Validate.IsEmail(request.Email, out emailError))
        {
            errorMessages = errorMessages.Append(emailError);
        }

        // Check phone
        if (request.Phone is not null
            && (!Validate.IsTextInRange(request.Phone, 10, 14, nameof(request.Phone), out var phoneError)
            || !Validate.HasOnlyDigits(request.Phone, nameof(request.Phone), out phoneError)))
        {
            errorMessages = errorMessages.Append(phoneError);
        }

        return !errorMessages.Any();
    }
}