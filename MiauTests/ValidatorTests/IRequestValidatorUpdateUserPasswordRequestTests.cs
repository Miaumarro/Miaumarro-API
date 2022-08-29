using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;

namespace MiauTests.ValidatorTests;

public sealed class IRequestValidatorUpdateUserPasswordRequestTests : BaseApiServiceTest
{
    private readonly IRequestValidator<UpdateUserPasswordRequest> _validator;

    public IRequestValidatorUpdateUserPasswordRequestTests(ServicesFixture fixture) : base(fixture)
        => _validator = base.Scope.ServiceProvider.GetRequiredService<IRequestValidator<UpdateUserPasswordRequest>>();

    [Theory]
    [InlineData(true, 0, "old", "new")]
    [InlineData(false, 1, "old", "")]
    [InlineData(false, 1, "", "new")]
    [InlineData(false, 2, "", "")]
    public void IsRequestValidTest(bool expected, int expectedErrorAmount, string oldPassword, string newPassword)
    {
        var request = new UpdateUserPasswordRequest(1, oldPassword, newPassword);

        Assert.Equal(expected, _validator.IsRequestValid(request, out var errorMessages));
        Assert.Equal(expectedErrorAmount, errorMessages.Count());
    }
}