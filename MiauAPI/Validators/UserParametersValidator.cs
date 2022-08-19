using MiauAPI.Models.QueryParameters;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="UserParameters"/>.
/// </summary>
public sealed class UserParametersValidator : IRequestValidator<UserParameters>
{
    public bool IsRequestValid(UserParameters request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        if (request.Ids.Length is 0 && request.Cpfs.Length is 0 && request.Emails.Length is 0)
            errorMessages = errorMessages.Append("At least one parameter is required.");

        return !errorMessages.Any();
    }
}