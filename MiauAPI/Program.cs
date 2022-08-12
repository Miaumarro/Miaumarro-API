using LinqToDB.EntityFrameworkCore;
using MiauAPI.Extensions;
using MiauDatabase.Extensions;

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
            .AddRouting()
            .AddMiauServices()
            .AddMiauDb()
            .AddMiauAuth(builder.Configuration.GetValue<byte[]>("Jwt:Key"));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}