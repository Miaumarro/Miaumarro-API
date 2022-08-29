using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;
using MiauDatabase.Enums;

namespace MiauTests.ValidatorTests;

public sealed class IRequestValidatorCreatedProductRequestTest : BaseApiServiceTest
{
    private readonly IRequestValidator<CreatedProductRequest> _validator;

    public IRequestValidatorCreatedProductRequestTest(ServicesFixture fixture) : base(fixture)
        => _validator = base.Scope.ServiceProvider.GetRequiredService<IRequestValidator<CreatedProductRequest>>();

    [Theory]
    [InlineData(true, 0, "Name", "Description", 10, true, 10, ProductTag.Accessory, "Brand", 0.1)]
    [InlineData(true, 0, "Name", "Description", 0.1, true, 0, ProductTag.None, null, 0)]
    [InlineData(true, 0, "N", "D", 10, false, 10, ProductTag.Accessory | ProductTag.Clothing, null, 1)]
    [InlineData(false, 1, "Name", "Description", 10, true, 10, ProductTag.Accessory, null, -0.1)]
    [InlineData(false, 2, "Name", "Description", 10, true, 10, ProductTag.Accessory, "", 1.1)]
    [InlineData(false, 3, "Name", "Description", 10, true, -1, ProductTag.Accessory, "", 1.1)]
    [InlineData(false, 4, "Name", "Description", -0.1, true, -1, ProductTag.Accessory, "", 1.1)]
    [InlineData(false, 5, "Name", "", -0.1, true, -1, ProductTag.Accessory, "", 1.1)]
    [InlineData(false, 6, "", "", -0.1, true, -1, ProductTag.Accessory, "", 1.1)]
    public void IsRequestValidTest(bool expected, int expectedAmount, string name, string description, decimal price, bool isActive, int amount, ProductTag tags, string? brand, decimal discount)
    {
        var request = new CreatedProductRequest(name, description, price, isActive, amount, tags, brand, discount);

        Assert.Equal(expected, _validator.IsRequestValid(request, out var errorMessages));
        Assert.Equal(expectedAmount, errorMessages.Count());
    }
}