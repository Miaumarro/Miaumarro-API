using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;
using MiauDatabase.Enums;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="CreatedProductRequest"/>.
/// </summary>
public sealed class CreatedProductRequestValidator : IRequestValidator<CreatedProductRequest>
{
    public bool IsRequestValid(CreatedProductRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();


        // Check Description
        if (!Validate.IsNullOrWhiteSpace(request.Description, nameof(request.Description), out var descriptionError)
            || !Validate.IsTextInRange(request.Description, 500, nameof(request.Description), out descriptionError))
        {
            errorMessages = errorMessages.Append(descriptionError);
        }

        // Check Price
        if (!Validate.IsPositive(request.Price, nameof(request.Price), out var priceError))
        {
            errorMessages = errorMessages.Append(priceError);
        }

        // Check Amount
        if (!Validate.IsPositive(request.Amount, nameof(request.Amount), out var amountError))
        {
            errorMessages = errorMessages.Append(amountError);
        }

        // Check Discount
        if (!Validate.IsValidDiscount(request.Discount, out var discountError))
        {
            errorMessages = errorMessages.Append(discountError);
        }

        // Check Brand
        if (!Validate.IsTextInRange(request.Brand, 30, nameof(request.Brand), out var brandError))
        {
            errorMessages = errorMessages.Append(brandError);
        }

        // Check Tags
        if (!Validate.IsValidEnum<ProductTag>(request.Tags, out var tagsError))
        {
            errorMessages = errorMessages.Append(tagsError);
        }

        return !errorMessages.Any();

    }
}