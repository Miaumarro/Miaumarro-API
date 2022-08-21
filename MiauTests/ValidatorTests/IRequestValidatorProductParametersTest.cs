using MiauAPI.Enums;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Validators.Abstractions;
using MiauDatabase.Enums;

namespace MiauTests.ValidatorTests;

public sealed class IRequestValidatorProductParametersTest : BaseApiServiceTest
{
    private readonly IRequestValidator<ProductParameters> _validator;

    public IRequestValidatorProductParametersTest(ServicesFixture fixture) : base(fixture)
        => _validator = base.Scope.ServiceProvider.GetRequiredService<IRequestValidator<ProductParameters>>();

    [Theory]
    [InlineData(true, 0, 0, 0, "search", "brand", false, ProductTag.None, SortParameter.DiscountAsc)]
    [InlineData(true, 0, 0, 0, null, null, true, ProductTag.None, SortParameter.DiscountAsc)]
    [InlineData(false, 2, 0, 0, "", "", false, ProductTag.None, SortParameter.DiscountAsc)]
    [InlineData(false, 1, -0.1, 0, null, null, true, ProductTag.None, SortParameter.DiscountAsc)]
    [InlineData(false, 2, -0.1, -0.1, null, null, true, ProductTag.None, SortParameter.DiscountAsc)]
    [InlineData(false, 1, 1, 0, null, null, true, ProductTag.None, SortParameter.DiscountAsc)]
    internal void IsRequestValidTest(bool expected, int expectedAmount, decimal minPrice, decimal maxPrice, string? searchTerm, string? brand, bool isDiscounted, ProductTag tags, SortParameter sort)
    {
        var request = new ProductParameters(minPrice, maxPrice, searchTerm, brand, isDiscounted, tags, sort);

        Assert.Equal(expected, _validator.IsRequestValid(request, out var errorMessages));
        Assert.Equal(expectedAmount, errorMessages.Count());
    }
}