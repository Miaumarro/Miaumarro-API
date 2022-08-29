using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="CreatedPurchaseRequest"/>.
/// </summary>
public class CreatedPurchaseRequestValidator : IRequestValidator<CreatedPurchaseRequest>
{
    public bool IsRequestValid(CreatedPurchaseRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        if (!Validate.IsPositive(request.UserId, nameof(request.UserId), out var idError)
            || !Validate.IsPositive(request.CouponId ?? 1, nameof(request.CouponId), out idError)
            || request.ProductIds.Any(x => x <= 0))
        {
            errorMessages = errorMessages.Append(idError ?? $"All {nameof(request.ProductIds)} must be higher than zero.");
        }

        if (request.ProductIds.Length is 0)
        {
            errorMessages = errorMessages.Append($"{nameof(request.ProductIds)} must not be empty.");
        }

        return !errorMessages.Any();
    }
}
