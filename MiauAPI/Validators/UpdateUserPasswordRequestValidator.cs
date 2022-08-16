using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="UpdateUserRequest"/>.
/// </summary>
public sealed class UpdateUserPasswordRequestValidator : IRequestValidator<UpdateUserPasswordRequest>
{
    public bool IsRequestValid(UpdateUserPasswordRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        // Check password
        if (Validate.IsNull(request.Password, nameof(request.Password), out var passwordError)
            || !Validate.IsTextInRange(request.Password, 100, nameof(request.Password), out passwordError))
        {
            errorMessages = errorMessages.Append(passwordError);
        }

        return !errorMessages.Any();
    }
}