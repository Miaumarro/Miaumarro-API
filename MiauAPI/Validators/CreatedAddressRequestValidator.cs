using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="CreatedAddressRequest"/>.
/// </summary>
public sealed class CreatedAddressRequestValidator : IRequestValidator<CreatedAddressRequest>
{
    public bool IsRequestValid(CreatedAddressRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        // Check Id
        if (!Validate.IsPositive(request.UserId, nameof(request.UserId), out var idError))
        {
            errorMessages = errorMessages.Append(idError);
        }

        // Check address
        if (Validate.IsNullOrWhiteSpace(request.Address, nameof(request.Address), out var addressError)
            || !Validate.IsTextInRange(request.Address, 60, nameof(request.Address), out addressError))
        {
            errorMessages = errorMessages.Append(addressError);
        }

        // Check number
        if (request.AddressNumber < 0)
        {
            errorMessages = errorMessages.Append($"{nameof(request.AddressNumber)} must be valid.");
        }

        // Check reference
        if (request.Reference is not null
            && !Validate.IsTextInRange(request.Reference, 100, nameof(request.Reference), out var referenceError))
        {
            errorMessages = errorMessages.Append(referenceError);
        }

        // Check complement
        if (request.Complement is not null
            && !Validate.IsTextInRange(request.Complement, 15, nameof(request.Complement), out var complementError))
        {
            errorMessages = errorMessages.Append(complementError);
        }

        // Check neighborhood
        if (Validate.IsNullOrWhiteSpace(request.Neighborhood, nameof(request.Neighborhood), out var neighborhoodError)
            || !Validate.IsTextInRange(request.Neighborhood, 30, nameof(request.Neighborhood), out neighborhoodError))
        {
            errorMessages = errorMessages.Append(neighborhoodError);
        }

        // Check city
        if (Validate.IsNullOrWhiteSpace(request.City, nameof(request.City), out var cityError)
            || !Validate.IsTextInRange(request.City, 30, nameof(request.City), out cityError))
        {
            errorMessages = errorMessages.Append(cityError);
        }

        // Check state
        if (Validate.IsNullOrWhiteSpace(request.State, nameof(request.State), out var stateError)
            || !Validate.IsTextInRange(request.State, 30, nameof(request.State), out stateError))
        {
            errorMessages = errorMessages.Append(stateError);
        }

        // Check destinatary
        if (request.Destinatary is not null
            && !Validate.IsTextInRange(request.Destinatary, 60, nameof(request.Destinatary), out var destinataryError))
        {
            errorMessages = errorMessages.Append(destinataryError);
        }

        // Check CEP
        if (Validate.IsNullOrWhiteSpace(request.Cep, nameof(request.Cep), out var cepError)
            || !Validate.IsTextInRange(request.Cep, 8, 8, nameof(request.Cep), out cepError)
            || !Validate.HasOnlyDigits(request.Cep, nameof(request.Cep), out cepError))
        {
            errorMessages = errorMessages.Append(cepError);
        }

        return !errorMessages.Any();
    }
}
