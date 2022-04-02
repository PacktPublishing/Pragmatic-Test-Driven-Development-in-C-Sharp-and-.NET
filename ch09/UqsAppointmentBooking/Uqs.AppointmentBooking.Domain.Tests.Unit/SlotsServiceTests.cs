using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;
using Uqs.AppointmentBooking.Domain.Services;
using Uqs.AppointmentBooking.Domain.Tests.Unit.Fakes;
using Xunit;

namespace Uqs.AppointmentBooking.Domain.Tests.Unit;

public class SlotsServiceTests
{
    private ApplicationContextFakeBuilder contextBuilder = new();
    private INowService _nowService = Substitute.For<INowService>();
    private SlotsService _sut;

    [Fact]
    public async Task GetAvailableSlots_NoShiftsForTomAndNoAppointmentsInSystem_NoSlots()
    {
        // Arrange
        DateTimeOffset appointmentFrom = new DateTimeOffset(2022, 10, 3, 7, 0, 0, TimeSpan.Zero);
        _nowService.Now.Returns(appointmentFrom);
        var context = contextBuilder
            .WithSingle30MinService()
            .WithSingleEmployeeTom()
            .Build();
        _sut = new SlotsService(context, _nowService);
        var tom = context.Employees!.Single();
        var mensCut30min = context.Services!.Single();

        // Act
        var slots = await _sut.GetAvailableSlotsForEmployee(mensCut30min.Id, tom.Id);

        // Assert
        var times = slots.DaysSlots.SelectMany(x => x.Times);
        Assert.Empty(times);
    }

    [Fact]
    public async Task GetAvailableSlots_OneShiftEqualsToServiceTimeAndNoExistingAppointments_OneSlotOnly()
    {
        // Arrange
        const int SERVICE_TIME_MIN = 30;
        DateTimeOffset shiftFrom = new DateTimeOffset(2022, 10, 3, 9, 0, 0, TimeSpan.Zero);
        DateTimeOffset shiftTo = shiftFrom.AddMinutes(SERVICE_TIME_MIN);
        DateTimeOffset appointmentFrom = new DateTimeOffset(2022, 10, 3, 7, 0, 0, TimeSpan.Zero);
        _nowService.Now.Returns(appointmentFrom);
        DateTimeOffset expectedSlotTime = new DateTimeOffset(2022, 10, 3, 9, 0, 0, TimeSpan.Zero);
        var context = contextBuilder
            .WithSingle30MinService()
            .WithSingleEmployeeTom()
            .WithSingleShiftForTom(shiftFrom, shiftTo)
            .Build();
        _sut = new SlotsService(context, _nowService);
        var tom = context.Employees!.Single();
        var mensCut30min = context.Services!.Single();

        // Act
        var slots = await _sut.GetAvailableSlotsForEmployee(mensCut30min.Id, tom.Id);

        // Assert
        var times = slots.DaysSlots.SelectMany(x => x.Times);
        var time = Assert.Single(times);
        Assert.Equal(expectedSlotTime, time);
    }
}
