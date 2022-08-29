using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauTests.ValidatorTests;

public sealed class IRequestValidatorUpdateUserRequestTests : BaseApiServiceTest
{
    private readonly IRequestValidator<UpdateUserRequest> _validator;

    public IRequestValidatorUpdateUserRequestTests(ServicesFixture fixture) : base(fixture)
        => _validator = base.Scope.ServiceProvider.GetRequiredService<IRequestValidator<UpdateUserRequest>>();

    [Theory]
    [InlineData(true, 0, "Name", "Surname", "test@email.com", "11111111111")]
    [InlineData(true, 0, "Name", "Surname", "test@email.com", null)]
    [InlineData(false, 1, "Name", "Surname", "test@email.com", "111111111")]
    [InlineData(false, 1, "Name", "Surname", "test@email.com", "111111111111111")]
    [InlineData(false, 1, "Name", "Surname", "testemail.com", null)]
    [InlineData(false, 1, "Name", "Surname", "", null)]
    [InlineData(false, 1, "Name", "", "test@email.com", null)]
    [InlineData(false, 1, "", "Surname", "test@email.com", null)]
    [InlineData(false, 3, "", "", "", null)]
    internal void IsRequestValidTest(bool expected, int expectedErrorAmount, string name, string surname, string email, string? phone)
    {
        var request = new UpdateUserRequest(1, name, surname, email, phone);

        Assert.Equal(expected, _validator.IsRequestValid(request, out var errorMessages));
        Assert.Equal(expectedErrorAmount, errorMessages.Count());
    }
}