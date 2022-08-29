using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using MiauAPI.Extensions;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using MiauDatabase;
using Microsoft.AspNetCore.Mvc;

namespace MiauTests.ServiceTests;

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
        var actionResult = await _service.RegisterUserAsync(request, "location/here/doesnt/matter");

        // Test the result
        Assert.IsType(expectedType, actionResult.Result);

        // If request succeeded, try to do it again
        if (actionResult.Result is CreatedResult)
        {
            var newActionResult = await _service.RegisterUserAsync(request, "location/here/doesnt/matter");
            Assert.IsType<BadRequestObjectResult>(newActionResult.Result);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    internal async Task RegisterUserFailTestAsync(string? location)
    {
        var validRequest = new CreatedUserRequest("12345678910", "Name", "Surname", "email1@test.com", "21999999999", "password");
        await Assert.ThrowsAsync<ArgumentException>(() => _service.RegisterUserAsync(validRequest, location!));
    }

    [Theory]
    // Good input
    [InlineData(typeof(OkObjectResult), 1, new[] { DefaultDbUser.Id }, new[] { DefaultDbUser.Cpf }, new[] { DefaultDbUser.Email })]
    [InlineData(typeof(OkObjectResult), 1, new[] { DefaultDbUser.Id }, new[] { DefaultDbUser.Cpf }, new string[] { })]
    [InlineData(typeof(OkObjectResult), 1, new[] { DefaultDbUser.Id }, new string[] { }, new string[] { })]
    [InlineData(typeof(OkObjectResult), 1, new int[] { }, new[] { DefaultDbUser.Cpf }, new[] { DefaultDbUser.Email })]
    [InlineData(typeof(OkObjectResult), 1, new int[] { }, new string[] { }, new[] { DefaultDbUser.Email })]
    [InlineData(typeof(OkObjectResult), 1, new int[] { }, new[] { DefaultDbUser.Cpf }, new string[] { })]
    [InlineData(typeof(OkObjectResult), 1, new[] { DefaultDbUser.Id }, new[] { "12345678999" }, new[] { "doesnt@exist.com" })]

    // Good input, but user doesn't exist
    [InlineData(typeof(NotFoundObjectResult), 0, new[] { 999 }, new string[] { }, new string[] { })]
    [InlineData(typeof(NotFoundObjectResult), 0, new int[] { }, new[] { "12345678999" }, new string[] { })]
    [InlineData(typeof(NotFoundObjectResult), 0, new int[] { }, new string[] { }, new[] { "doesnt@exist.com" })]

    // Bad input
    [InlineData(typeof(BadRequestObjectResult), 0, new int[] { }, new string[] { }, new string[] { })]
    internal async Task GetUsersTestAsync(Type expectedType, int usersAmount, int[] ids, string[] cpfs, string[] emails)
    {
        var request = new UserParameters(ids, cpfs, emails);
        var result = await _service.GetUsersAsync(request);

        Assert.IsType(expectedType, result.Result);

        if (result.Result.TryUnwrap<PagedResponse>(out var response))
            Assert.Equal(usersAmount, response.Amount);
    }

    [Fact]
    internal async Task DeleteUsersTestAsync()
    {
        // Create temporary user
        var (createRequest, _) = await RegisterTemporaryUserAsync();

        // Delete temporary user
        var deleteRequest = new UserParameters(Array.Empty<int>(), new[] { createRequest.Cpf }, new[] { createRequest.Email });
        var deleteResult = await _service.DeleteUsersAsync(deleteRequest);

        Assert.IsType<OkObjectResult>(deleteResult.Result);

        // Check if temporary user was deleted
        var getUsersResult = await _service.GetUsersAsync(deleteRequest);

        Assert.IsType<NotFoundObjectResult>(getUsersResult.Result);

        // Try to delete again
        var deleteAgainResult = await _service.DeleteUsersAsync(deleteRequest);

        Assert.IsType<NotFoundObjectResult>(deleteAgainResult.Result);
    }

    [Fact]
    internal async Task UpdateUserTestAsync()
    {
        // Create temporary user
        var (createRequest, createResponse) = await RegisterTemporaryUserAsync();

        // Update the user
        var updateRequest = new UpdateUserRequest(createResponse.Id, "NewName", "NewSurname", createRequest.Email, createRequest.Phone);
        var updateResult = await _service.UpdateUserAsync(updateRequest);

        Assert.IsType<OkResult>(updateResult.Result);

        // Check if the user got updated
        var db = base.Scope.ServiceProvider.GetRequiredService<MiauDbContext>();
        var updatedDbUser = await db.Users.FirstAsyncEF(x => x.Cpf == createRequest.Cpf);

        Assert.Equal(updateRequest.Name, updatedDbUser.Name);
        Assert.Equal(updateRequest.Surname, updatedDbUser.Surname);

        // Cleanup
        await db.Users.DeleteAsync(x => x.Id == updateRequest.Id);
    }

    [Fact]
    internal async Task UpdateUserPasswordTestAsync()
    {
        // Create temporary user
        var (createRequest, createResponse) = await RegisterTemporaryUserAsync();

        // Update the password
        var updateRequest = new UpdateUserPasswordRequest(createResponse.Id, createRequest.Password, "avocado");
        var updateResult = await _service.UpdateUserPasswordAsync(updateRequest);

        Assert.IsType<OkResult>(updateResult.Result);

        // Cleanup
        var db = base.Scope.ServiceProvider.GetRequiredService<MiauDbContext>();
        await db.Users.DeleteAsync(x => x.Id == updateRequest.Id);
    }

    /// <summary>
    /// Creates a temporary user in the database with random credentials.
    /// </summary>
    /// <returns>The creation request and response.</returns>
    private async Task<(CreatedUserRequest, UserAuthenticationResponse)> RegisterTemporaryUserAsync()
    {
        // Create temporary user
        var createRequest = new CreatedUserRequest(Generate.RandomCpf(), "Name", "Surname", Generate.RandomEmail(), null, "password");
        var createResult = await _service.RegisterUserAsync(createRequest, "location/doesnt/matter");

        Assert.IsType<CreatedResult>(createResult.Result);

        return (createRequest, (UserAuthenticationResponse)((CreatedResult)createResult.Result!).Value!);
    }
}