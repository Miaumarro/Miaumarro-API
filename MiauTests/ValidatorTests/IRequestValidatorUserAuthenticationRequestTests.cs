using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauTests.ValidatorTests;

public sealed class IRequestValidatorUserAuthenticationRequestTests : BaseApiServiceTest
{
    private readonly IRequestValidator<UserAuthenticationRequest> _validator;

    public IRequestValidatorUserAuthenticationRequestTests(ServicesFixture fixture) : base(fixture)
        => _validator = base.Scope.ServiceProvider.GetRequiredService<IRequestValidator<UserAuthenticationRequest>>();

    [Theory]
    [InlineData(true, 0, "12345678910", "user@email.com", "password")]
    [InlineData(true, 0, "", "user@email.com", "password")]
    [InlineData(true, 0, null, "user@email.com", "password")]
    [InlineData(true, 0, "12345678910", "", "password")]
    [InlineData(true, 0, "12345678910", null, "password")]
    [InlineData(true, 0, "12345678910", "user@emailcom", "password")]
    [InlineData(true, 0, "123456789110", "user@email.com", "password")]
    [InlineData(false, 1, "12345678910", "user@email.com", "")]
    [InlineData(false, 1, "12345678910", "user@email.com", null)]
    [InlineData(false, 1, "", "user@email.com", null)]
    [InlineData(false, 1, null, "user@email.com", null)]
    [InlineData(false, 4, null, "", null)]
    [InlineData(false, 3, null, null, null)]
    internal void IsRequestValidTest(bool expected, int expectedErrorAmount, string? cpf, string? email, string password)
    {
        var request = new UserAuthenticationRequest(cpf, email, password);

        Assert.Equal(expected, _validator.IsRequestValid(request, out var errorMessages));
        Assert.Equal(expectedErrorAmount, errorMessages.Count());
    }
}