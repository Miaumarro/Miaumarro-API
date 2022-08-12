using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="UserAuthenticationRequest"/>.
/// </summary>
public sealed class UserAuthenticationRequestValidator : IRequestValidator<UserAuthenticationRequest>
{
    public bool IsRequestValid(UserAuthenticationRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        // Check both CPF and e-mail
        if (Validate.IsNull(request.Cpf, nameof(request.Cpf), out var nullError)
            || Validate.IsNull(request.Email, nameof(request.Email), out nullError))
        {
            errorMessages = errorMessages.Append(nullError);
        }

        // Check e-mail
        if (!Validate.IsNull(request.Email, nameof(request.Email), out _)
            && (!Validate.IsTextInRange(request.Email, 60, nameof(request.Email), out var emailError)
            || !Validate.IsEmail(request.Email!, out emailError)))
        {
            errorMessages = errorMessages.Append(emailError);
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