using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;
using MiauDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiauTests.ValidatorTests;
public sealed class IRequestValidatorCreatedPurchaseRequestTest : BaseApiServiceTest
{
    private readonly IRequestValidator<CreatedPurchaseRequest> _validator;

    public IRequestValidatorCreatedPurchaseRequestTest(ServicesFixture fixture) : base(fixture)
        => _validator = base.Scope.ServiceProvider.GetRequiredService<IRequestValidator<CreatedPurchaseRequest>>();

    [Theory]
    [InlineData(true, 0, 1, 1, new[] { 1, 2, 3, 4, 5 })]
    [InlineData(true, 0, 1, null, new[] { 1 })]
    [InlineData(false, 1, 1, 1, new int[] { })]
    [InlineData(false, 2, 1, 0, new int[] { })]
    [InlineData(false, 2, 0, 0, new int[] { })]
    internal void IsRequestValidTest(bool expected, int expectedAmount, int userId, int? couponId, int[] productIds)
    {
        var request = new CreatedPurchaseRequest(userId, productIds, couponId);

        Assert.Equal(expected, _validator.IsRequestValid(request, out var errorMessages));
        Assert.Equal(expectedAmount, errorMessages.Count());
    }
}
