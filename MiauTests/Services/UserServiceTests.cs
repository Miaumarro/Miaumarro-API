using MiauAPI.Models.Requests;
using MiauAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiauTests.Services;

public sealed class UserServiceTests : BaseApiServiceTest
{
    private readonly UserService _service;

    public UserServiceTests(ServicesFixture fixture) : base(fixture)
        => _service = base.Scope.ServiceProvider.GetRequiredService<UserService>();

    [Theory]
    // Good input
    [InlineData(typeof(CreatedResult), "12345678910", "Name", "Surname", "email1@test.com", "21999999999", "password")]
    [InlineData(typeof(CreatedResult), "12345678911", "Name", "Surname", "email2@test.com", null, "password")]

    // Bad CPF
    [InlineData(typeof(BadRequestObjectResult), "", "Name", "Surname", "email3@test.com", null, "password")]
    [InlineData(typeof(BadRequestObjectResult), "123", "Name", "Surname", "email4@test.com", null, "password")]
    [InlineData(typeof(BadRequestObjectResult), "1234567890a", "Name", "Surname", "email5@test.com", null, "password")]
    [InlineData(typeof(BadRequestObjectResult), "12345 78901", "Name", "Surname", "email6@test.com", null, "password")]

    // Bad name
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "", "Surname", "email7@test.com", null, "password")]

    // Bad surname
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "Name", "", "email8@test.com", null, "password")]

    // Bad e-mail
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "Name", "Surname", "", null, "password")]
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "Name", "Surname", "@", null, "password")]
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "Name", "Surname", "email@", null, "password")]
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "Name", "Surname", "@test.com", null, "password")]
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "Name", "Surname", "email@testcom", null, "password")]
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "Name", "Surname", "emailtest.com", null, "password")]

    // Bad phone
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "Name", "Surname", "email9@test.com", "", "password")]
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "Name", "Surname", "email10@test.com", "123456789", "password")]
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "Name", "Surname", "email11@test.com", "1234567891012141", "password")]
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "Name", "Surname", "email12@test.com", "1234567890a", "password")]
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "Name", "Surname", "email13@test.com", "123456 7890", "password")]

    // Bad password
    [InlineData(typeof(BadRequestObjectResult), "12345678901", "Name", "Surname", "email14@test.com", null, "")]
    internal async Task RegisterUserTestAsync(Type expectedType, string cpf, string name, string surname, string email, string? phone, string password)
    {
        // Create the request
        var request = new CreatedUserRequest(cpf, name, surname, email, phone, password);

        // Use the service
        var actionResult = await _service.CreateUserAsync(request, "location/here/doesnt/matter");

        // Test the result
        Assert.IsType(expectedType, actionResult.Result);

        // If request succeeded, try to do it again
        if (actionResult.Result is CreatedResult)
        {
            var newActionResult = await _service.CreateUserAsync(request, "location/here/doesnt/matter");
            Assert.IsType<BadRequestObjectResult>(newActionResult.Result);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    internal async Task RegisterUserFailTestAsync(string? location)
    {
        var validRequest = new CreatedUserRequest("12345678910", "Name", "Surname", "email1@test.com", "21999999999", "password");
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateUserAsync(validRequest, location!));
    }
}