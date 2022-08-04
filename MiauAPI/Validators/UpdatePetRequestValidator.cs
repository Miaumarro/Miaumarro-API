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

        // Check name
        if (Validate.IsNull(request.Name, nameof(request.Name), out var nameError)
            || !Validate.IsTextInRange(request.Name, 30, nameof(request.Name), out nameError))
        {
            errorMessages = errorMessages.Append(nameError);
        }

        // Check breed
        if (!Validate.IsTextInRange(request.Breed, 30, nameof(request.Breed), out var breedError))
        {
            errorMessages = errorMessages.Append(breedError);
        }

        // Check DateOfBirth
        if (!Validate.IsDateValid(request.DateOfBirth, nameof(request.DateOfBirth), out var dateOfBirthError))
        {
            errorMessages = errorMessages.Append(dateOfBirthError);
        }

        return !errorMessages.Any();
    }
}

