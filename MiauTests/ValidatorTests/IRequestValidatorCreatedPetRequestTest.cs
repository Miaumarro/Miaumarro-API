using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;
using MiauDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MiauTests.ValidatorTests;
public class IRequestValidatorCreatedPetRequestTest : BaseApiServiceTest
{
    private readonly IRequestValidator<CreatedPetRequest> _validator;

    public IRequestValidatorCreatedPetRequestTest(ServicesFixture fixture) : base(fixture)
        => _validator = base.Scope.ServiceProvider.GetRequiredService<IRequestValidator<CreatedPetRequest>>();

    [Theory]
    [InlineData(true, 0, 1, "Name", -0.1, "Breed")]
    [InlineData(true, 0, 1, "Name", -0.1, null)]
    [InlineData(false, 1, 1, "Name", 0.1, null)]
    [InlineData(false, 2, 1, "", 0.1, null)]
    [InlineData(false, 3, 0, "", 0.1, null)]
    internal void IsRequestValidTest(bool expected, int expectedAmount, int id, string name, double addHours, string? breed)
    {
        var request = new CreatedPetRequest(id, name, PetType.Dog, PetGender.Male, DateTime.UtcNow.AddHours(addHours), breed, null);

        Assert.Equal(expected, _validator.IsRequestValid(request, out var errorMessages));
        Assert.Equal(expectedAmount, errorMessages.Count());
    }
}
