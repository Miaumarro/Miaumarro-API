using MiauAPI.Models.Requests;
using MiauAPI.Services;
using MiauAPI.Validators;
using MiauAPI.Validators.Abstractions;
using MiauDatabase.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

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
            .AddScoped<ProductService>()
            .AddScoped<ProductImageService>()
            .AddScoped<PetService>()
            .AddScoped<AddressService>()
            .AddScoped<AppointmentService>()
            .AddScoped<AuthenticationService>()
            .AddSingleton<IRequestValidator<CreatedUserRequest>, CreatedUserRequestValidator>()
            .AddSingleton<IRequestValidator<UpdateUserRequest>, UpdateUserRequestValidator>()
            .AddSingleton<IRequestValidator<CreatedProductRequest>, CreatedProductRequestValidator>()
            .AddSingleton<IRequestValidator<UpdateProductRequest>, UpdateProductRequestValidator>()
            .AddSingleton<IRequestValidator<CreatedPetRequest>, CreatedPetRequestValidator>()
            .AddSingleton<IRequestValidator<UpdatePetRequest>, UpdatePetRequestValidator>()
            .AddSingleton<IRequestValidator<CreatedAddressRequest>, CreatedAddressRequestValidator>() 
            .AddSingleton<IRequestValidator<UpdateAddressRequest>, UpdateAddressRequestValidator>()
            .AddSingleton<IRequestValidator<CreatedAppointmentRequest>, CreatedAppointmentRequestValidator>()
            .AddSingleton<IRequestValidator<UpdateAppointmentRequest>, UpdateAppointmentRequestValidator>();
    }

    /// <summary>
    /// Adds the default authentication and authorization policies used by the MiauAPI.
    /// </summary>
    /// <param name="serviceCollection">This service collection.</param>
    /// <param name="privateKey">The private key to be used for signature validation.</param>
    /// <param name="addSwaggerAuth"><see langword="true"/> to add an authentication button on Swagger, <see langword="false"/> otherwise.</param>
    /// <returns>This service collection with the policies added.</returns>
    public static IServiceCollection AddMiauAuth(this IServiceCollection serviceCollection, byte[] privateKey, bool addSwaggerAuth = true)
    {
        if (addSwaggerAuth)
        {
            serviceCollection.AddSwaggerGen(x => // Add Swagger authentication button
            {
                var apiSecurityScheme = new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string in the format: `bearer jwt-token-here`",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                x.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, apiSecurityScheme);

                // OpenApiSecurityRequirement is a Dictionary<OpenApiSecurityScheme, IList<string>>
                // Any scheme other than "oauth2" or "openIdConnect" must contain an empty IList
                x.AddSecurityRequirement(new OpenApiSecurityRequirement() { [apiSecurityScheme] = Array.Empty<string>() });
            });
        }

        serviceCollection
            .AddAuthorization(x =>
            {
                // Require all users to be authenticated
                x.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // Add authorization policies
                // Based on the name of UserPermissions enums (except "Blocked")
                foreach (var permission in Enum.GetValues<UserPermissions>().Where(x => x is not UserPermissions.Blocked))
                    x.AddPolicy(permission.ToString(), policy => policy.RequireClaim(ClaimTypes.Role, permission.ToString()));
            })
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(privateKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        return serviceCollection;
    }
}