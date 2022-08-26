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

        if (request.UserId <= 0 || request.CouponId <= 0 || request.ProductsId.Any(x => x <= 0))
        {
            errorMessages = errorMessages.Append($"Ids must be higher than zero.");
        }

        if (request.ProductsId.Length is 0)
        {
            errorMessages = errorMessages.Append($"{nameof(request.ProductsId)} must not be empty.");
        }

        return !errorMessages.Any();
    }
}
