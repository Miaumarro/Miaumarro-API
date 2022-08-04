using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="CreatedWishlistItemRequest"/>.
/// </summary>
public sealed class CreatedWishlistItemRequestValidator : IRequestValidator<CreatedWishlistItemRequest>
{
    public bool IsRequestValid(CreatedWishlistItemRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        //Check user
        if (Validate.IsNull(request.UserId, nameof(request.UserId), out var userError))
        {
            errorMessages = errorMessages.Append(userError);
        }

        //Check product
        if (Validate.IsNull(request.ProductId, nameof(request.ProductId), out var productError))
        {
            errorMessages = errorMessages.Append(productError);
        }

        return !errorMessages.Any();
    }
}