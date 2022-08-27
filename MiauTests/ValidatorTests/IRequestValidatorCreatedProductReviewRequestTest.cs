using MiauAPI.Enums;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;
using MiauDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiauTests.ValidatorTests;
public sealed class IRequestValidatorCreatedProductReviewRequestTest : BaseApiServiceTest
{
    private readonly IRequestValidator<CreatedProductReviewRequest> _validator;

    public IRequestValidatorCreatedProductReviewRequestTest(ServicesFixture fixture) : base(fixture)
        => _validator = base.Scope.ServiceProvider.GetRequiredService<IRequestValidator<CreatedProductReviewRequest>>();

    [Theory]
    [InlineData(true, 0, 1, 1, "Description", 0)]
    [InlineData(true, 0, null, 1, "Description", 5)]
    [InlineData(false, 1, 1, 1, "Description", -1)]
    [InlineData(false, 1, 1, 1, "Description", 6)]
    [InlineData(false, 2, 1, 1, "", 6)]
    [InlineData(false, 3, 1, 0, "", 6)]
    [InlineData(false, 4, 0, 0, "", 6)]
    internal void IsRequestValidTest(bool expected, int expectedAmount, int? userId, int productId, string description, int score)
    {
        var request = new CreatedProductReviewRequest(userId, productId, description, score);

        Assert.Equal(expected, _validator.IsRequestValid(request, out var errorMessages));
        Assert.Equal(expectedAmount, errorMessages.Count());
    }
}
