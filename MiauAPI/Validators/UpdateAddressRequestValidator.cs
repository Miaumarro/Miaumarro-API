using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Validators;

/// <summary>
/// Validates a <see cref="UpdateAddressRequest"/>.
/// </summary>
public sealed class UpdateAddressRequestValidator : IRequestValidator<UpdateAddressRequest>
{
    public bool IsRequestValid(UpdateAddressRequest request, out IEnumerable<string> errorMessages)
    {
        errorMessages = Enumerable.Empty<string>();

        // Check Ids
        if (request.Id <= 0 || request.UserId <= 0)
        {
            errorMessages = errorMessages.Append($"{nameof(request.Id)} and {request.UserId} must be greater than zero.");
        }

        // Check address
        if (Validate.IsNullOrWhiteSpace(request.Address, nameof(request.Address), out var addressError)
            || !Validate.IsTextInRange(request.Address, 60, nameof(request.Address), out addressError))
        {
            errorMessages = errorMessages.Append(addressError);
        }

        // Check number
        if (!Validate.IsPositive(request.AddressNumber, nameof(request.AddressNumber), out var numberError))
        {
            errorMessages = errorMessages.Append(numberError);
        }

        // Check reference
        if (request.Reference is not null &&
            !Validate.IsTextInRange(request.Reference, 100, nameof(request.Reference), out var referenceError))
        {
            errorMessages = errorMessages.Append(referenceError);
        }

        // Check complement
        if (request.Complement is not null &&
            !Validate.IsTextInRange(request.Complement, 15, nameof(request.Complement), out var complementError))
        {
            errorMessages = errorMessages.Append(complementError);
        }

        // Check neighborhood
        if (Validate.IsNull(request.Neighborhood, nameof(request.Neighborhood), out var neighborhoodError)
            || !Validate.IsTextInRange(request.Neighborhood, 30, nameof(request.Neighborhood), out neighborhoodError))
        {
            errorMessages = errorMessages.Append(neighborhoodError);
        }

        // Check city
        if (Validate.IsNull(request.City, nameof(request.City), out var cityError)
            || !Validate.IsTextInRange(request.City, 30, nameof(request.City), out cityError))
        {
            errorMessages = errorMessages.Append(cityError);
        }

        // Check state
        if (Validate.IsNull(request.State, nameof(request.State), out var stateError)
            || !Validate.IsTextInRange(request.State, 30, nameof(request.State), out stateError))
        {
            errorMessages = errorMessages.Append(stateError);
        }

        // Check destinatary
        if (request.Destinatary != null && 
            !Validate.IsTextInRange(request.Destinatary, 60, nameof(request.Destinatary), out var destinataryError))
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
