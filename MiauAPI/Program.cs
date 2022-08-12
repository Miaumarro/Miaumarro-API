using LinqToDB.EntityFrameworkCore;
using MiauAPI.Extensions;
using MiauDatabase.Enums;
using MiauDatabase.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace MiauAPI;

public class Program
{
    public static void Main(string[] args)
    {
        // Enable LinqToDb extensions
        LinqToDBForEFTools.Initialize();

        // Build web api
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // Add IoC services
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services
            .AddEndpointsApiExplorer()
            .AddMiauServices()
            .AddMiauDb()
            .AddAuthorization(x =>
            {
                // Require all users to be authenticated
                x.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // Add authorization policies
                // Based on the name of UserPermissions enums (except "Blocked")
                foreach (var value in Enum.GetValues<UserPermissions>().Where(x => x is not UserPermissions.Blocked))
                    x.AddPolicy(value.ToString(), policy => policy.RequireClaim(ClaimTypes.Role, value.ToString()));
            })
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(builder.Configuration.GetValue<byte[]>("Jwt:Key")),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        builder.Services
            .AddSwaggerGen(x => // Add Swagger authentication button
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
            })
            .AddRouting();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}