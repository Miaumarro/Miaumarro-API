using MiauAPI.Models.QueryParameters;
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
            .AddScoped<AddressService>()
            .AddScoped<AppointmentService>()
            .AddScoped<AuthenticationService>()
            .AddScoped<PetService>()
            .AddScoped<ProductImageService>()
            .AddScoped<ProductReviewService>()
            .AddScoped<ProductService>()
            .AddScoped<PurchaseService>()
            .AddScoped<UserService>()
            .AddScoped<WishlistService>()
            .AddSingleton<ImageService>()
            .AddSingleton<IRequestValidator<CreatedAddressRequest>, CreatedAddressRequestValidator>()
            .AddSingleton<IRequestValidator<CreatedAddressRequest>, CreatedAddressRequestValidator>()
            .AddSingleton<IRequestValidator<CreatedAppointmentRequest>, CreatedAppointmentRequestValidator>()
            .AddSingleton<IRequestValidator<CreatedPetRequest>, CreatedPetRequestValidator>()
            .AddSingleton<IRequestValidator<CreatedProductRequest>, CreatedProductRequestValidator>()
            .AddSingleton<IRequestValidator<CreatedUserRequest>, CreatedUserRequestValidator>()
            .AddSingleton<IRequestValidator<ProductParameters>, ProductParametersValidator>()
            .AddSingleton<IRequestValidator<UpdateAddressRequest>, UpdateAddressRequestValidator>()
            .AddSingleton<IRequestValidator<UpdateAppointmentRequest>, UpdateAppointmentRequestValidator>()
            .AddSingleton<IRequestValidator<UpdatePetRequest>, UpdatePetRequestValidator>()
            .AddSingleton<IRequestValidator<UpdateProductRequest>, UpdateProductRequestValidator>()
            .AddSingleton<IRequestValidator<UpdateUserPasswordRequest>, UpdateUserPasswordRequestValidator>()
            .AddSingleton<IRequestValidator<UpdateUserRequest>, UpdateUserRequestValidator>()
            .AddSingleton<IRequestValidator<UserParameters>, UserParametersValidator>(); ;
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
            // Add Swagger authentication button
            serviceCollection.AddSwaggerGen(x =>
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
            .AddSingleton<IRequestValidator<UserAuthenticationRequest>, UserAuthenticationRequestValidator>()
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