using MiauAPI.Services;
using MiauDatabase.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MiauTests.ServiceTests;

public sealed class AuthenticationServiceTests : BaseApiServiceTest
{
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests(ServicesFixture fixture) : base(fixture)
        => _service = base.Scope.ServiceProvider.GetRequiredService<AuthenticationService>();

    [Theory]
    [InlineData(typeof(OkObjectResult), DefaultDbUser.Cpf, DefaultDbUser.Email, DefaultDbUser.Password)]
    [InlineData(typeof(OkObjectResult), DefaultDbUser.Cpf, null, DefaultDbUser.Password)]
    [InlineData(typeof(OkObjectResult), null, DefaultDbUser.Email, DefaultDbUser.Password)]
    [InlineData(typeof(NotFoundObjectResult), "11111111111", "doesnt@exist.com", DefaultDbUser.Password)]
    [InlineData(typeof(NotFoundObjectResult), DefaultDbUser.Cpf, DefaultDbUser.Email, "banana")]
    [InlineData(typeof(NotFoundObjectResult), DefaultDbUser.Cpf, null, "banana")]
    [InlineData(typeof(NotFoundObjectResult), null, DefaultDbUser.Email, "banana")]
    [InlineData(typeof(BadRequestObjectResult), null, null, DefaultDbUser.Password)]
    [InlineData(typeof(BadRequestObjectResult), null, null, "banana")]
    [InlineData(typeof(BadRequestObjectResult), DefaultDbUser.Cpf, DefaultDbUser.Email, "")]
    [InlineData(typeof(BadRequestObjectResult), DefaultDbUser.Cpf, DefaultDbUser.Email, null)]
    internal async Task LoginUserTestAsync(Type expectedType, string? cpf, string? email, string password)
    {
        // Use the service
        var actionResult = await _service.LoginUserAsync(new(cpf, email, password));

        // Test the result
        Assert.IsType(expectedType, actionResult.Result);
    }

    [Fact]
    internal void GenerateSessionTokenUserTest()
    {
        var token = _service.GenerateSessionToken(DefaultDbUser.Instance, DateTime.UtcNow.AddDays(1));
        Assert.NotEmpty(token);
    }

    [Theory]
    [InlineData(DefaultDbUser.Name, DefaultDbUser.Email, UserPermissions.Blocked, 7)]
    [InlineData(DefaultDbUser.Name, DefaultDbUser.Email, UserPermissions.Customer, 1)]
    [InlineData(DefaultDbUser.Name, DefaultDbUser.Email, UserPermissions.Customer | UserPermissions.Clerk, 0.5)]
    [InlineData(DefaultDbUser.Name, DefaultDbUser.Email, UserPermissions.Customer | UserPermissions.Clerk | UserPermissions.Administrator, 0.1)]
    internal void GenerateSessionTokenTest(string name, string email, UserPermissions permission, double days)
    {
        var token = _service.GenerateSessionToken(name, email, permission, DateTime.UtcNow.AddDays(days));
        Assert.NotEmpty(token);
    }

    [Theory]
    [InlineData("", DefaultDbUser.Email, 1)]
    [InlineData(DefaultDbUser.Name, "", 1)]
    [InlineData("", "", 1)]
    [InlineData(null, DefaultDbUser.Email, 1)]
    [InlineData(DefaultDbUser.Name, null, 1)]
    [InlineData(null, null, 1)]
    [InlineData(DefaultDbUser.Name, DefaultDbUser.Email, 0)]
    [InlineData(DefaultDbUser.Name, DefaultDbUser.Email, -1)]
    internal void GenerateSessionTokenFailTest(string name, string email, double days)
        => Assert.Throws<ArgumentException>(() => _service.GenerateSessionToken(name, email, default, DateTime.UtcNow.AddDays(days)));

    [Theory]
    [InlineData("", DefaultDbUser.Email, 1)]
    [InlineData(DefaultDbUser.Name, "", 1)]
    [InlineData("", "", 1)]
    [InlineData(null, DefaultDbUser.Email, 1)]
    [InlineData(DefaultDbUser.Name, null, 1)]
    [InlineData(null, null, 1)]
    [InlineData(DefaultDbUser.Name, DefaultDbUser.Email, 0)]
    [InlineData(DefaultDbUser.Name, DefaultDbUser.Email, -1)]
    internal void GenerateSessionTokenUserFailTest(string name, string email, double days)
    {
        var user = DefaultDbUser.Instance with
        {
            Name = name,
            Email = email,
        };

        Assert.Throws<ArgumentException>(() => _service.GenerateSessionToken(user, DateTime.UtcNow.AddDays(days)));
    }
}