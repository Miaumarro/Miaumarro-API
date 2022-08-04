using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="UpdateProductRequest"/>.
/// </summary>
public sealed class UpdateProductRequestValidator : IRequestValidator<UpdateProductRequest>
{
    public bool IsRequestValid(UpdateProductRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        // Check Name
        if (!Validate.IsNullOrWhiteSpace(request.Name, nameof(request.Name), out var nameError)
            || !Validate.IsTextInRange(request.Name, 50, nameof(request.Name), out nameError))
        {
            errorMessages = errorMessages.Append(nameError);
        }

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
        if (!Validate.IsWithinRange(request.Discount, 0, 1, nameof(request.Discount), out var discountError))
        {
            errorMessages = errorMessages.Append(discountError);
        }

        // Check Brand
        if (!Validate.IsTextInRange(request.Brand, 30, nameof(request.Brand), out var brandError))
        {
            errorMessages = errorMessages.Append(brandError);
        }

        return !errorMessages.Any();

    }
}