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

        if (request.MinPrice < 0)
            errorMessages = errorMessages.Append($"{nameof(request.MinPrice)} cannot be negative.");

        if (request.MaxPrice < 0)
            errorMessages = errorMessages.Append($"{nameof(request.MaxPrice)} cannot be negative.");

        if (request.MinPrice > request.MaxPrice)
            errorMessages = errorMessages.Append($"{nameof(request.MinPrice)} must be lower or equal to {nameof(request.MaxPrice)}");

        return !errorMessages.Any();
    }
}