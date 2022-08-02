using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="ProductImageRequest"/>.
/// </summary>
public sealed class ProductImageRequestValidator : IRequestValidator<ProductImageRequest>
{
    public bool IsRequestValid(ProductImageRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        //Check Product
        if (Validate.IsNull(request.ProductId, nameof(request.ProductId), out var productError))
        {
            errorMessages = errorMessages.Append(productError);
        }

        return !errorMessages.Any();

    }
}