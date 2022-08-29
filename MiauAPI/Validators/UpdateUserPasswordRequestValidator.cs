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

        // Check old password
        if (Validate.IsNullOrWhiteSpace(request.OldPassword, nameof(request.OldPassword), out var oldPasswordError)
            || !Validate.IsTextInRange(request.OldPassword, 100, nameof(request.OldPassword), out oldPasswordError))
        {
            errorMessages = errorMessages.Append(oldPasswordError);
        }

        // Check new password
        if (Validate.IsNullOrWhiteSpace(request.NewPassword, nameof(request.NewPassword), out var newPasswordError)
            || !Validate.IsTextInRange(request.NewPassword, 100, nameof(request.NewPassword), out newPasswordError))
        {
            errorMessages = errorMessages.Append(newPasswordError);
        }

        return !errorMessages.Any();
    }
}