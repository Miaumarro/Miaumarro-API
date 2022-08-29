using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="CreatedProductImageRequest"/>.
/// </summary>
public sealed class ProductImageRequestValidator : IRequestValidator<CreatedProductImageRequest>
{
    public bool IsRequestValid(CreatedProductImageRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        //Check Product
        if (!Validate.IsPositive(request.ProductId, nameof(request.ProductId), out var productError))
        {
            errorMessages = errorMessages.Append(productError);
        }

        return !errorMessages.Any();
    }
}