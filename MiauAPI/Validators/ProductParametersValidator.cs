using MiauAPI.Models.QueryParameters;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

public sealed class ProductParametersValidator : IRequestValidator<ProductParameters>
{
    public bool IsRequestValid(ProductParameters request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        if (request.SearchedTerm == string.Empty)
            errorMessages = errorMessages.Append("Search term cannot be empty.");

        if (request.Brand == string.Empty)
            errorMessages = errorMessages.Append("Brand cannot be empty.");

        if (!Validate.IsPositiveOrNeutral(request.MinPrice, nameof(request.MinPrice), out var minError))
            errorMessages = errorMessages.Append(minError);

        if (!Validate.IsPositiveOrNeutral(request.MaxPrice, nameof(request.MaxPrice), out var maxError))
            errorMessages = errorMessages.Append(maxError);

        if (request.MaxPrice is not 0 && request.MinPrice > request.MaxPrice)
            errorMessages = errorMessages.Append($"{nameof(request.MinPrice)} must be lower or equal to {nameof(request.MaxPrice)}");

        return !errorMessages.Any();
    }
}