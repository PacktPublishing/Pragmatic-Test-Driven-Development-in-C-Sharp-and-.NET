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
            .WithSingle30MinService()
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

    [Fact]
    public async Task GetAvailableSlots_OneShiftEqualsToServiceTimeAndNoExistingAppointments_OneSlotOnly()
    {
        // Arrange
        const int SERVICE_TIME_MIN = 30;
        DateTime shiftFrom = new DateTime(2022, 10, 3, 9, 0, 0);
        DateTime shiftTo = shiftFrom.AddMinutes(SERVICE_TIME_MIN);
        DateTime appointmentFrom = new DateTime(2022, 10, 3, 7, 0, 0);
        _nowService.Now.Returns(appointmentFrom);
        DateTime expectedSlotTime = new DateTime(2022, 10, 3, 9, 0, 0);
        var context = _contextBuilder
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

    [Fact]
    public async Task GetAvailableSlots_OneShiftEqualsToServiceTimePlusIntervalAndNoExistingAppointments_OneSlotOnly()
    {
        // Arrange
        const int SERVICE_TIME_MIN = 35;
        DateTime shiftFrom = new DateTime(2022, 10, 3, 9, 0, 0);
        DateTime shiftTo = shiftFrom.AddMinutes(SERVICE_TIME_MIN);
        DateTime appointmentFrom = new DateTime(2022, 10, 3, 7, 0, 0);
        _nowService.Now.Returns(appointmentFrom);
        DateTime expectedSlotTime = new DateTime(2022, 10, 3, 9, 0, 0);
        var context = _contextBuilder
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
        var times = slots.DaysSlots.SelectMany(x => x.Times).ToArray();
        Assert.Equal(2, times.Count());
        Assert.Equal(shiftFrom, times[0]);
        Assert.Equal(shiftFrom.AddMinutes(5), times[1]);
    }
}
