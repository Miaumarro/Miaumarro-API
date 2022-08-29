using MiauAPI.Models.QueryParameters;
using MiauAPI.Validators.Abstractions;

namespace MiauTests.ValidatorTests;

public sealed class IRequestValidatorUserParametersTests : BaseApiServiceTest
{
    private readonly IRequestValidator<UserParameters> _validator;

    public IRequestValidatorUserParametersTests(ServicesFixture fixture) : base(fixture)
        => _validator = base.Scope.ServiceProvider.GetRequiredService<IRequestValidator<UserParameters>>();

    [Theory]
    [InlineData(true, 0, new[] { 1, 2, 3 }, new[] { "11111111111", "22222222222", "3333333333" }, new[] { "test1@email.com", "test2@email.com", "test3@email.com" })]
    [InlineData(true, 0, new[] { 1, 2, 3 }, new[] { "11111111111" }, new[] { "test1@email.com" })]
    [InlineData(true, 0, new[] { 1 }, new[] { "11111111111" }, new string[] { })]
    [InlineData(true, 0, new[] { 1 }, new string[] { }, new string[] { })]
    [InlineData(true, 0, new int[] { }, new[] { "11111111111" }, new string[] { })]
    [InlineData(true, 0, new int[] { }, new string[] { }, new[] { "test1@email.com" })]
    [InlineData(false, 1, new int[] { }, new string[] { }, new string[] { })]
    internal void IsRequestValidTest(bool expected, int expectedErrorAmount, int[] ids, string[] cpfs, string[] emails)
    {
        var request = new UserParameters(ids, cpfs, emails);

        Assert.Equal(expected, _validator.IsRequestValid(request, out var errorMessages));
        Assert.Equal(expectedErrorAmount, errorMessages.Count());
    }
}