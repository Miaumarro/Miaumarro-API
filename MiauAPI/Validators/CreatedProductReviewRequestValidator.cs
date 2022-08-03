using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="CreatedProductReviewRequest"/>.
/// </summary>
public sealed class CreatedProductReviewRequestValidator : IRequestValidator<CreatedProductReviewRequest>
{
    public bool IsRequestValid(CreatedProductReviewRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        //Check product
        if (Validate.IsNull(request.ProductId, nameof(request.ProductId), out var productError))
        {
            errorMessages = errorMessages.Append(productError);
        }

        return !errorMessages.Any();
    }
}