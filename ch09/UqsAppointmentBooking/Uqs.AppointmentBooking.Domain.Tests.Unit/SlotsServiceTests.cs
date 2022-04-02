using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;
using Uqs.AppointmentBooking.Domain.Services;
using Uqs.AppointmentBooking.Domain.Tests.Unit.Fakes;
using Xunit;

namespace Uqs.AppointmentBooking.Domain.Tests.Unit;

public class SlotsServiceTests : IDisposable
{
    private readonly ApplicationContextFakeBuilder _contextBuilder = new();
    private readonly INowService _nowService = Substitute.For<INowService>();
    private SlotsService _sut;

    public void Dispose()
    {
        _contextBuilder.Dispose();
    }

    [Fact]
    public async Task GetAvailableSlots_NoShiftsForTomAndNoAppointmentsInSystem_NoSlots()
    {
        // Arrange
        DateTime appointmentFrom = new DateTime(2022, 10, 3, 7, 0, 0);
        _nowService.Now.Returns(appointmentFrom);
        var context = _contextBuilder
            .WithSingleService(30)
            .WithSingleEmployeeTom()
            .Build();
        _sut = new SlotsService(context, _nowService);
        var tom = context.Employees!.Single();
        var mensCut30Min = context.Services!.Single();

        // Act
        var slots = await _sut.GetAvailableSlotsForEmployee(mensCut30Min.Id, tom.Id);

        // Assert
        var times = slots.DaysSlots.SelectMany(x => x.Times);
        Assert.Empty(times);
    }

    [Theory]
    [InlineData(5, 0)]
    [InlineData(25, 0)]
    [InlineData(30, 1, "2022-10-03 09:00:00")]
    [InlineData(35, 2, "2022-10-03 09:00:00", "2022-10-03 09:05:00")]
    public async Task GetAvailableSlots_OneShiftAndNoExistingAppointments_VaryingSlots(
        int serviceDuration, int totalSlots, params string[] expectedTimes)
    {
        // Arrange
        DateTime shiftFrom = new DateTime(2022, 10, 3, 9, 0, 0);
        DateTime shiftTo = shiftFrom.AddMinutes(serviceDuration);
        DateTime appointmentFrom = new DateTime(2022, 10, 3, 7, 0, 0);
        _nowService.Now.Returns(appointmentFrom);
        var context = _contextBuilder
            .WithSingleService(30)
            .WithSingleEmployeeTom()
            .WithSingleShiftForTom(shiftFrom, shiftTo)
            .Build();
        _sut = new SlotsService(context, _nowService);
        var tom = context.Employees!.Single();
        var mensCut30min = context.Services!.Single();

        // Act
        var slots = await _sut.GetAvailableSlotsForEmployee(mensCut30min.Id, tom.Id);

        // Assert
        var times = slots.DaysSlots.SelectMany(x => x.Times).ToArray();
        Assert.Equal(totalSlots, times.Length);
        for (int i = 0;i < expectedTimes.Length; i++)
        {
            var time = times[i];
            Assert.Equal(DateTime.Parse(expectedTimes[i]), time);
        }
    }

}