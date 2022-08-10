using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="CreatedPurchaseRequest"/>.
/// </summary>
public sealed class CreatedPurchaseRequestValidator : IRequestValidator<CreatedPurchaseRequest>
{
    public bool IsRequestValid(CreatedPurchaseRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        //Check user
        if (Validate.IsNull(request.UserId, nameof(request.UserId), out var userError))
        {
            errorMessages = errorMessages.Append(userError);
        }

        return !errorMessages.Any();
    }
}
