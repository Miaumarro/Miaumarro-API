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

        // Check Description
        if (!Validate.IsNullOrWhiteSpace(request.Description, nameof(request.Description), out var descriptionError)
            || !Validate.IsTextInRange(request.Description, 500, nameof(request.Description), out descriptionError))
        {
            errorMessages = errorMessages.Append(descriptionError);
        }

        return !errorMessages.Any();
    }
}
