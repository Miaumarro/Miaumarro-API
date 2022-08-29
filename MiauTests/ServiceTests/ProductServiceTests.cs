using MiauAPI.Models.Requests;
using MiauAPI.Services;
using MiauDatabase.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MiauTests.ServiceTests;

public sealed class ProductServiceTests : BaseApiServiceTest
{
    private readonly ProductService _service;

    public ProductServiceTests(ServicesFixture fixture) : base(fixture)
        => _service = base.Scope.ServiceProvider.GetRequiredService<ProductService>();

    [Theory]
    [InlineData(typeof(CreatedResult), "Name", "Description", 10, true, 10, ProductTag.Accessory, "Brand", 0.1)]
    [InlineData(typeof(CreatedResult), "Name", "Description", 10, false, 10, ProductTag.None, null, 0)]
    [InlineData(typeof(BadRequestObjectResult), "Name", "Description", 10, true, 10, ProductTag.None, "Brand", -0.1)]
    [InlineData(typeof(BadRequestObjectResult), "Name", "Description", 10, true, 10, ProductTag.None, "", 0)]
    [InlineData(typeof(BadRequestObjectResult), "Name", "Description", 10, true, -1, ProductTag.Accessory, "Brand", 0)]
    [InlineData(typeof(BadRequestObjectResult), "Name", "Description", -0.1, true, 10, ProductTag.Accessory, "Brand", 0)]
    [InlineData(typeof(BadRequestObjectResult), "Name", "", 10, true, 10, ProductTag.Accessory, "Brand", 0)]
    [InlineData(typeof(BadRequestObjectResult), "", "Description", 10, true, 10, ProductTag.Accessory, "Brand", 0)]
    internal async Task CreateProductTestAsync(Type expectedType, string name, string description, decimal price, bool isActive, int amount, ProductTag tags, string? brand, decimal discount)
    {
        var request = new CreatedProductRequest(name, description, price, isActive, amount, tags, brand, discount);
        var actionResult = await _service.CreateProductAsync(request, "location/doesnt/matter");

        Assert.IsType(expectedType, actionResult.Result);
    }

    /*
    * TODO: test the following methods
    *
    * GetProductsAsync
    * GetProductByIdAsync
    * DeleteProductByIdAsync
    * UpdateProductByIdAsync
    *
    */
}