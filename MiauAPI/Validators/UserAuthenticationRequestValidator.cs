using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="UserAuthenticationRequest"/>.
/// </summary>
public sealed class UserAuthenticationRequestValidator : IRequestValidator<UserAuthenticationRequest>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Occurs when <paramref name="request"/> is <see langword="null"/>.</exception>
    public bool IsRequestValid(UserAuthenticationRequest request, out IEnumerable<string> errorMessages)
    {
        ArgumentNullException.ThrowIfNull(request);

        errorMessages = Enumerable.Empty<string>();

        // Check password
        if (Validate.IsNullOrWhiteSpace(request.Password, nameof(request.Password), out var passwordError)
            || !Validate.IsTextInRange(request.Password, 100, nameof(request.Password), out passwordError))
        {
            errorMessages = errorMessages.Append(passwordError);
        }

        // Check both CPF and e-mail
        if (Validate.IsNullOrWhiteSpace(request.Cpf, nameof(request.Cpf), out var invalidCpfError)
            && Validate.IsNullOrWhiteSpace(request.Email, nameof(request.Email), out var invalidEmailError))
        {
            errorMessages = errorMessages.Append(invalidCpfError).Append(invalidEmailError);
        }

        // Check e-mail
        if (request.Email is not null && string.IsNullOrWhiteSpace(request.Cpf)
            && (!Validate.IsTextInRange(request.Email, 60, nameof(request.Email), out var emailError)
            || !Validate.IsEmail(request.Email, out emailError)))
        {
            errorMessages = errorMessages.Append(emailError);
        }

        return !errorMessages.Any();
    }
}