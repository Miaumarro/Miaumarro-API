using MiauAPI.Models.Requests;
using MiauAPI.Services;
using MiauAPI.Validators;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Extensions;

/// <summary>
/// Contains extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExt
{
    /// <summary>
    /// Adds all services used by the MiauAPI.
    /// </summary>
    /// <param name="serviceCollection">This service collection.</param>
    /// <returns>This service collection, with the services added.</returns>
    public static IServiceCollection AddMiauServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddScoped<UserService>()
            .AddScoped<PetService>()
            .AddSingleton<IRequestValidator<CreatedUserRequest>, CreatedUserRequestValidator>()
            .AddSingleton<IRequestValidator<CreatedPetRequest>, CreatedPetRequestValidator>();
    }
}