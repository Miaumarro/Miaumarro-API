using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using MiauAPI.Common;
using MiauAPI.Extensions;
using MiauDatabase.Extensions;

namespace MiauAPI;

public class Program
{
    public static void Main(string[] args)
    {
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
            .AddMiauAuth(builder.Configuration.GetValue<byte[]>(ApiConstants.JwtAppSetting));

        var app = builder.Build();

        // Enable LinqToDb extensions and enable its logging
        LinqToDBForEFTools.Initialize();
        DataConnection.TurnTraceSwitchOn();
        DataConnection.WriteTraceLine = (message, displayName, traceLevel)
            => app.Logger.Log(traceLevel.ToLogLevel(), new(10403, displayName), message ?? string.Empty, null, (state, ex) => state);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection()
            .UseRouting()
            .UseCors(x =>
            x.AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod()
            )
            .UseAuthentication()
            .UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}