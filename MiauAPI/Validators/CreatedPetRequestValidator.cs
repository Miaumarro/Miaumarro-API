using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="CreatedPetRequest"/>.
/// </summary>
public sealed class CreatedPetRequestValidator : IRequestValidator<CreatedPetRequest>
{
    public bool IsRequestValid(CreatedPetRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        //Check User
        if (Validate.IsNull(request.UserId, nameof(request.UserId), out var userError))
        {
            errorMessages = errorMessages.Append(userError);
        }
        

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
        if (Validate.IsNull(request.DateOfBirth, nameof(request.DateOfBirth), out var dateOfBirthError) 
            || !Validate.IsDateValid(request.DateOfBirth, nameof(request.DateOfBirth), out dateOfBirthError))
        {
            errorMessages = errorMessages.Append(dateOfBirthError);
        }

        return !errorMessages.Any();
    }
}