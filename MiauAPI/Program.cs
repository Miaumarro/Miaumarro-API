using LinqToDB.EntityFrameworkCore;
using MiauAPI.Extensions;
using MiauDatabase.Enums;
using MiauDatabase.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
            .AddSwaggerGen()
            .AddMiauServices()
            .AddMiauDb()
            .AddAuthorization(x =>
            {
                foreach (var value in Enum.GetValues<UserPermissions>().Where(x => x is not UserPermissions.Blocked))
                    x.AddPolicy(value.ToString(), policy => policy.RequireClaim(nameof(MiauAPI), value.ToString()));
            })
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(builder.Configuration.GetValue<byte[]>("Jwt:Key")),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
                    ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience")
                };
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        //app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}