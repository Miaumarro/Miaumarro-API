using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="CreatedUserRequest"/>.
/// </summary>
public sealed class CreatedUserRequestValidator : IRequestValidator<CreatedUserRequest>
{
    public bool IsRequestValid(CreatedUserRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        // Check CPF
        if (Validate.IsNull(request.Cpf, nameof(request.Cpf), out var cpfError)
            || !Validate.IsTextInRange(request.Cpf, 11, 11, nameof(request.Cpf), out cpfError)
            || !Validate.HasOnlyDigits(request.Cpf, nameof(request.Cpf), out cpfError))
        {
            errorMessages = errorMessages.Append(cpfError);
        }

        // Check name
        if (Validate.IsNull(request.Name, nameof(request.Name), out var nameError)
            || !Validate.IsTextInRange(request.Name, 30, nameof(request.Name), out nameError))
        {
            errorMessages = errorMessages.Append(nameError);
        }

        // Check surname
        if (Validate.IsNull(request.Surname, nameof(request.Surname), out var surnameError)
            || !Validate.IsTextInRange(request.Surname, 60, nameof(request.Surname), out surnameError))
        {
            errorMessages = errorMessages.Append(surnameError);
        }

        // Check e-mail
        if (Validate.IsNull(request.Email, nameof(request.Email), out var emailError)
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

        // Check password
        if (Validate.IsNull(request.Password, nameof(request.Password), out var passwordError)
            || !Validate.IsTextInRange(request.Password, 100, nameof(request.Password), out passwordError))
        {
            errorMessages = errorMessages.Append(passwordError);
        }

        return !errorMessages.Any();
    }
}