using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="CreatedPetRequest"/>.
/// </summary>
public sealed class CreatedPetRequestValidator : IRequestValidator<CreatedPetRequest>
{
    /*private readonly MiauDbContext _db;

    public CreatedPetRequestValidator(MiauDbContext db)
    {
        _db = db;
    }*/
    public bool IsRequestValid(CreatedPetRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        //Check User
        /*
        if (Validate.IsNull(request.User, nameof(request.User), out var userError)
            || !Validate.UserExistsInDatabase(request.User, db, out userError))
        {
            errorMessages = errorMessages.Append(userError);
        }
        */

        // Check name
        if (Validate.IsNull(request.Name, nameof(request.Name), out var nameError)
            || !Validate.IsTextInRange(request.Name, 30, nameof(request.Name), out nameError))
        {
            errorMessages = errorMessages.Append(nameError);
        }

        // Check PetType
        if (Validate.IsNull(request.Type, nameof(request.Type), out var typeError)) 
            //Falta adicionar função que verifica se o tipo é válido no Enum PetType
        {
            errorMessages = errorMessages.Append(typeError);
        }

        // Check PetGender
        if (Validate.IsNull(request.Gender, nameof(request.Gender), out var genderError)) 
            //Falta adicionar função que verifica se o gênero é válido no Enum PetGender
        {
            errorMessages = errorMessages.Append(genderError);
        }

        // Check breed
        if (!Validate.IsTextInRange(request.Breed, 30, nameof(request.Breed), out var breedError))
        {
            errorMessages = errorMessages.Append(breedError);
        }

        // Check ImageFileUrl
        if (!Validate.IsTextInRange(request.ImageFileUrl, 256, nameof(request.ImageFileUrl), out var imageFileUrlError))
            //Acho que precisa adicionar um validador para verificar se o url é válido, mas não sei como fazer
        {
            errorMessages = errorMessages.Append(imageFileUrlError);
        }

        // Check DateOfBirth
        if (Validate.IsNull(request.DateOfBirth, nameof(request.DateOfBirth), out var dateOfBirthError))
        //Acho que precisa adicionar um validador para verificar se a data é válida (se não é no futuro ou se o pet tem mais de 30 anos, não sei)
        {
            errorMessages = errorMessages.Append(dateOfBirthError);
        }

        return !errorMessages.Any();
    }
}