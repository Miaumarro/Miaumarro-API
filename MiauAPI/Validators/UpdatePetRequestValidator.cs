using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="UpdatePetRequest"/>.
/// </summary>
public sealed class UpdatePetRequestValidator : IRequestValidator<UpdatePetRequest>
{
    public bool IsRequestValid(UpdatePetRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        // Check ids
        if (request.Id <= 0 || request.UserId <= 0)
        {
            errorMessages = errorMessages.Append($"{nameof(request.Id)} and {nameof(request.UserId)} must be valid.");
        }

        // Check name
        if (Validate.IsNullOrWhiteSpace(request.Name, nameof(request.Name), out var nameError)
            || !Validate.IsTextInRange(request.Name, 30, nameof(request.Name), out nameError))
        {
            errorMessages = errorMessages.Append(nameError);
        }

        // Check breed
        if (request.Breed is not null
            && !Validate.IsTextInRange(request.Breed, 30, nameof(request.Breed), out var breedError))
        {
            errorMessages = errorMessages.Append(breedError);
        }

        // Check DateOfBirth
        if (!Validate.IsDateInFuture(request.DateOfBirth, nameof(request.DateOfBirth), out var dateOfBirthError))
        {
            errorMessages = errorMessages.Append(dateOfBirthError);
        }

        // Check image
        if (request.Image is not null && request.Image.Length > 2000000)
        {
            errorMessages = errorMessages.Append($"{nameof(request.Image)} cannot be greater than 2 MB.");
        }

        return !errorMessages.Any();
    }
}

