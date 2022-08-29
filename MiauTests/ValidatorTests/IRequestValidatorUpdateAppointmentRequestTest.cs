using MiauAPI.Models.Requests;
using MiauAPI.Validators.Abstractions;
using MiauDatabase.Enums;

namespace MiauTests.ValidatorTests;

public sealed class IRequestValidatorUpdateAppointmentRequestTest : BaseApiServiceTest
{
    private readonly IRequestValidator<UpdateAppointmentRequest> _validator;

    public IRequestValidatorUpdateAppointmentRequestTest(ServicesFixture fixture) : base(fixture)
        => _validator = base.Scope.ServiceProvider.GetRequiredService<IRequestValidator<UpdateAppointmentRequest>>();

    [Theory]
    [InlineData(true, 0, 1, 1, 0.1, AppointmentType.Bath, 0.1)]
    [InlineData(true, 0, 1, 1, 0, AppointmentType.Grooming, 0.1)]
    [InlineData(false, 1, 1, 1, 0.1, AppointmentType.Bath, 0)]
    [InlineData(false, 2, 1, 1, -0.1, AppointmentType.Bath, 0)]
    [InlineData(false, 3, 1, 0, -0.1, AppointmentType.Bath, 0)]
    [InlineData(false, 3, 0, 0, -0.1, AppointmentType.Bath, 0)]
    [InlineData(false, 3, 0, 1, -0.1, AppointmentType.Bath, 0)]
    internal void IsRequestValidTest(bool expected, int expectedAmount, int id, int petId, decimal price, AppointmentType appType, double minutesIntoTheFuture)
    {
        var request = new UpdateAppointmentRequest(id, petId, price, appType, DateTime.UtcNow.AddMinutes(minutesIntoTheFuture));

        Assert.Equal(expected, _validator.IsRequestValid(request, out var errorMessages));
        Assert.Equal(expectedAmount, errorMessages.Count());
    }
}