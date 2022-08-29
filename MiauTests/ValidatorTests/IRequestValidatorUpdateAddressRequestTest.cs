using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;
using System;

namespace MiauTests.ValidatorTests;

public sealed class IRequestValidatorUpdateAddressRequestTest : BaseApiServiceTest
{
    private readonly IRequestValidator<UpdateAddressRequest> _validator;

    public IRequestValidatorUpdateAddressRequestTest(ServicesFixture fixture) : base(fixture)
        => _validator = base.Scope.ServiceProvider.GetRequiredService<IRequestValidator<UpdateAddressRequest>>();

    [Theory]
    [InlineData(true, 0, "ST", "City", "Neigh", "11111111", "Address", 1, "Compl", "Ref", "Def")]
    [InlineData(true, 0, "ST", "City", "Neigh", "11111111", "Address", 1, null, null, null)]
    [InlineData(false, 1, "ST", "City", "Neigh", "11111111", "Address", -1, null, null, null)]
    [InlineData(false, 2, "ST", "City", "Neigh", "11111111", "", -1, null, null, null)]
    [InlineData(false, 3, "ST", "City", "Neigh", "111a1111", "", -1, null, null, null)]
    [InlineData(false, 3, "ST", "City", "Neigh", "", "", -1, null, null, null)]
    [InlineData(false, 4, "ST", "City", "", "", "", -1, null, null, null)]
    [InlineData(false, 5, "ST", "", "", "", "", -1, null, null, null)]
    [InlineData(false, 6, "", "", "", "", "", -1, null, null, null)]
    [InlineData(false, 9, "", "", "", "", "", -1, "", "", "")]
    public void IsRequestValidTest(bool expected, int expectedAmount, string state, string city, string neighborhood, string cep, string address, int addressNumber, string? complement, string? reference, string? destinatary)
    {
        var request = new UpdateAddressRequest(1, 1, state, city, neighborhood, cep, address, addressNumber, complement, reference, destinatary);

        Assert.Equal(expected, _validator.IsRequestValid(request, out var errorMessages));
        Assert.Equal(expectedAmount, errorMessages.Count());
    }
}