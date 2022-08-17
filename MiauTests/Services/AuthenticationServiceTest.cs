using MiauAPI.Services;
using MiauDatabase;
using MiauDatabase.Entities;
using MiauDatabase.Enums;
using Microsoft.AspNetCore.Mvc;
using Encrypt = BCrypt.Net.BCrypt;

namespace MiauTests.Services;

public sealed class AuthenticationServiceTest : BaseApiServiceTest
{
    private const string _defaultCpf = "12345678920";
    private const string _defaultEmail = "user@email.com";
    private const string _defaultPassword = "avocado";
    private static readonly object _lockObject = new();
    private static bool _firstRun = true;
    private readonly AuthenticationService _service;
    private static readonly UserEntity _defaultUser = new()
    {
        Cpf = _defaultCpf,
        Name = "Herp",
        Surname = "Derp",
        Email = _defaultEmail,
        HashedPassword = Encrypt.HashPassword(_defaultPassword)
    };

    public AuthenticationServiceTest(ServicesFixture fixture) : base(fixture)
    {
        _service = base.Scope.ServiceProvider.GetRequiredService<AuthenticationService>();

        // Create one user once for the entire test run
        // Since this class is instantiated once per test
        // and tests run in parallel, we must watch out
        // for race conditions.
        lock (_lockObject)
        {
            if (!_firstRun)
                return;

            var db = base.Scope.ServiceProvider.GetRequiredService<MiauDbContext>();

            db.Users.Add(_defaultUser);
            db.SaveChanges();

            _firstRun = false;
        }
    }

    [Theory]
    [InlineData(typeof(OkObjectResult), _defaultCpf, _defaultEmail, _defaultPassword)]
    [InlineData(typeof(OkObjectResult), _defaultCpf, null, _defaultPassword)]
    [InlineData(typeof(OkObjectResult), null, _defaultEmail, _defaultPassword)]
    [InlineData(typeof(NotFoundObjectResult), "11111111111", "doesnt@exist.com", _defaultPassword)]
    [InlineData(typeof(NotFoundObjectResult), _defaultCpf, _defaultEmail, "banana")]
    [InlineData(typeof(NotFoundObjectResult), _defaultCpf, null, "banana")]
    [InlineData(typeof(NotFoundObjectResult), null, _defaultEmail, "banana")]
    [InlineData(typeof(BadRequestObjectResult), null, null, _defaultPassword)]
    [InlineData(typeof(BadRequestObjectResult), null, null, "banana")]
    [InlineData(typeof(BadRequestObjectResult), _defaultCpf, _defaultEmail, "")]
    [InlineData(typeof(BadRequestObjectResult), _defaultCpf, _defaultEmail, null)]
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
        var token = _service.GenerateSessionToken(_defaultUser, DateTime.UtcNow.AddDays(1));
        Assert.NotEmpty(token);
    }

    [Theory]
    [InlineData("Herp", _defaultEmail, UserPermissions.Blocked, 7)]
    [InlineData("Herp", _defaultEmail, UserPermissions.Customer, 1)]
    [InlineData("Herp", _defaultEmail, UserPermissions.Customer | UserPermissions.Clerk, 0.5)]
    [InlineData("Herp", _defaultEmail, UserPermissions.Customer | UserPermissions.Clerk | UserPermissions.Administrator, 0.1)]
    internal void GenerateSessionTokenTest(string name, string email, UserPermissions permission, double days)
    {
        var token = _service.GenerateSessionToken(name, email, permission, DateTime.UtcNow.AddDays(days));
        Assert.NotEmpty(token);
    }

    [Theory]
    [InlineData("", _defaultEmail, 1)]
    [InlineData("Herp", "", 1)]
    [InlineData("", "", 1)]
    [InlineData(null, _defaultEmail, 1)]
    [InlineData("Herp", null, 1)]
    [InlineData(null, null, 1)]
    [InlineData("Herp", _defaultEmail, 0)]
    [InlineData("Herp", _defaultEmail, -1)]
    internal void GenerateSessionTokenFailTest(string name, string email, double days)
        => Assert.Throws<ArgumentException>(() => _service.GenerateSessionToken(name, email, default, DateTime.UtcNow.AddDays(days)));

    [Theory]
    [InlineData("", _defaultEmail, 1)]
    [InlineData("Herp", "", 1)]
    [InlineData("", "", 1)]
    [InlineData(null, _defaultEmail, 1)]
    [InlineData("Herp", null, 1)]
    [InlineData(null, null, 1)]
    [InlineData("Herp", _defaultEmail, 0)]
    [InlineData("Herp", _defaultEmail, -1)]
    internal void GenerateSessionTokenUserFailTest(string name, string email, double days)
    {
        var user = _defaultUser with
        {
            Name = name,
            Email = email,
        };

        Assert.Throws<ArgumentException>(() => _service.GenerateSessionToken(user, DateTime.UtcNow.AddDays(days)));
    }
}