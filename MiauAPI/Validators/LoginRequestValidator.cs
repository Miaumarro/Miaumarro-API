using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="LoginUserRequest"/>.
/// </summary>
public sealed class LoginRequestValidator : IRequestValidator<LoginUserRequest>
{
    public bool IsRequestValid(LoginUserRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();


        // Check e-mail
        if (Validate.IsNull(request.Email, nameof(request.Email), out var emailError)
            || !Validate.IsTextInRange(request.Email, 60, nameof(request.Email), out emailError)
            || !Validate.IsEmail(request.Email, out emailError))
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