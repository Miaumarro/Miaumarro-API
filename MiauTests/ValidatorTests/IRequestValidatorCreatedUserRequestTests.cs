using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauTests.ValidatorTests;

public sealed class IRequestValidatorCreatedUserRequestTests : BaseApiServiceTest
{
    private readonly IRequestValidator<CreatedUserRequest> _validator;

    public IRequestValidatorCreatedUserRequestTests(ServicesFixture fixture) : base(fixture)
        => _validator = base.Scope.ServiceProvider.GetRequiredService<IRequestValidator<CreatedUserRequest>>();

    [Theory]
    [InlineData(true, 0, "12345678910", "Name", "Surname", "email@test.com", "1112345678910", "password")]
    [InlineData(true, 0, "12345678910", "Name", "Surname", "email@test.com", null, "password")]
    [InlineData(false, 1, "12", "Name", "Surname", "email@test.com", "1112345678910", "password")]
    [InlineData(false, 2, "12", "", "Surname", "email@test.com", "1112345678910", "password")]
    [InlineData(false, 3, "12", "", "", "email@test.com", "1112345678910", "password")]
    [InlineData(false, 4, "12", "", "", "emailtest.com", "1112345678910", "password")]
    [InlineData(false, 5, "12", "", "", "emailtest.com", "12345678910_0", "password")]
    [InlineData(false, 6, "12", "", "", "emailtest.com", "12345678910_0", "")]
    [InlineData(false, 6, null!, null!, null!, null!, "12345678910_0", null!)]
    public void IsRequestValidTest(bool expected, int expectedErrorAmount, string cpf, string name, string surname, string email, string? phone, string password)
    {
        var request = new CreatedUserRequest(cpf, name, surname, email, phone, password);

        Assert.Equal(expected, _validator.IsRequestValid(request, out var errorMessages));
        Assert.Equal(expectedErrorAmount, errorMessages.Count());
    }
}