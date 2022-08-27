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

        // Check User Id
        if (request.UserId is not null and not > 0)
        {
            errorMessages = errorMessages.Append($"{nameof(request.UserId)} is not valid.");
        }

        // Check Product Id
        if (!Validate.IsPositive(request.ProductId, nameof(request.ProductId), out var productIdError))
        {
            errorMessages = errorMessages.Append(productIdError);
        }

        // Check Description
        if (Validate.IsNullOrWhiteSpace(request.Description, nameof(request.Description), out var descriptionError)
            || !Validate.IsTextInRange(request.Description, 500, nameof(request.Description), out descriptionError))
        {
            errorMessages = errorMessages.Append(descriptionError);
        }

        // Check Score
        if (!Validate.IsWithinRange(request.Score, 0, 5, nameof(request.Score), out var scoreError))
        {
            errorMessages = errorMessages.Append(scoreError);
        }

        return !errorMessages.Any();
    }
}

