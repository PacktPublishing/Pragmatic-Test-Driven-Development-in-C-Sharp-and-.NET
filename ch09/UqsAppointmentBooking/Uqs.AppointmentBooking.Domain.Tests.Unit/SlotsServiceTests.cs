using Microsoft.Extensions.Options;
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
    private readonly ApplicationSettings _applicationSettings = 
        new ApplicationSettings {
        OpenAppointmentInDays = 7, RoundUpInMin = 5, RestInMin = 5 };
    private readonly IOptions<ApplicationSettings> _settings = 
        Substitute.For<IOptions<ApplicationSettings>>();
    private SlotsService? _sut;

    public SlotsServiceTests()
    {
        _settings.Value.Returns(_applicationSettings);
    }

    public void Dispose()
    {
        _contextBuilder.Dispose();
    }

    [Fact]
    public async Task GetAvailableSlotsForEmployee_NoShiftsForTomAndNoAppointmentsInSystem_NoSlots()
    {
        // Arrange
        DateTime appointmentFrom = new DateTime(2022, 10, 3, 7, 0, 0);
        _nowService.Now.Returns(appointmentFrom);
        var context = _contextBuilder
            .WithSingleService(30)
            .WithSingleEmployeeTom()
            .Build();
        _sut = new SlotsService(context, _nowService, _settings);
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
    public async Task GetAvailableSlotsForEmployee_OneShiftAndNoExistingAppointments_VaryingSlots(
        int serviceDuration, int totalSlots, params string[] expectedTimes)
    {
        // Arrange
        DateTime timeOfBooking = new DateTime(2022, 10, 3, 7, 0, 0);
        _nowService.Now.Returns(timeOfBooking);
        DateTime shiftFrom = new DateTime(2022, 10, 3, 9, 0, 0);
        DateTime shiftTo = shiftFrom.AddMinutes(serviceDuration);
        var context = _contextBuilder
            .WithSingleService(30)
            .WithSingleEmployeeTom()
            .WithSingleShiftForTom(shiftFrom, shiftTo)
            .Build();
        _sut = new SlotsService(context, _nowService, _settings);
        var tom = context.Employees!.Single();
        var mensCut30Min = context.Services!.Single();

        // Act
        var slots = await _sut.GetAvailableSlotsForEmployee(mensCut30Min.Id, tom.Id);

        // Assert
        var times = slots.DaysSlots.SelectMany(x => x.Times).ToArray();
        Assert.Equal(totalSlots, times.Length);
        for (int i = 0;i < expectedTimes.Length; i++)
        {
            var time = times[i];
            Assert.Equal(DateTime.Parse(expectedTimes[i]), time);
        }
    }

    [Theory]
    [InlineData("2022-10-03 09:00:00", "2022-10-03 11:10:00", 0)]
    [InlineData("2022-10-03 09:30:00", "2022-10-03 11:10:00", 0)]
    [InlineData("2022-10-03 09:00:00", "2022-10-03 10:45:00", 0)]
    [InlineData("2022-10-03 09:35:00", "2022-10-03 11:10:00", 1, "2022-10-03 09:00:00")]
    [InlineData("2022-10-03 09:40:00", "2022-10-03 11:10:00", 2, "2022-10-03 09:00:00", "2022-10-03 09:05:00")]
    [InlineData("2022-10-03 09:00:00", "2022-10-03 10:30:00", 2, "2022-10-03 10:35:00", "2022-10-03 10:40:00")]
    [InlineData("2022-10-03 09:35:00", "2022-10-03 10:30:00", 3, "2022-10-03 09:00:00", "2022-10-03 10:35:00", "2022-10-03 10:40:00")]
    public async Task GetAvailableSlotsForEmployee_OneShiftWithVaryingAppointments_VaryingSlots(
        string appointmentStartStr, string appointmentEndStr, int totalSlots, params string[] expectedTimes)
    {
        // Arrange
        DateTime timeOfBooking = new DateTime(2022, 10, 3, 7, 0, 0); // 07:00
        _nowService.Now.Returns(timeOfBooking);
        DateTime shiftFrom = new DateTime(2022, 10, 3, 9, 0, 0); // 09:00
        DateTime shiftTo = new DateTime(2022, 10, 3, 11, 10, 0); // 11:10
        DateTime appointmentStart = DateTime.Parse(appointmentStartStr);
        DateTime appointmentEnd = DateTime.Parse(appointmentEndStr);
        var context = _contextBuilder
            .WithSingleService(30)
            .WithSingleEmployeeTom()
            .WithSingleShiftForTom(shiftFrom, shiftTo)
            .WithSingleCustomerPaul()
            .WithSingleAppointmentForTom(appointmentStart, appointmentEnd)
            .Build();
        _sut = new SlotsService(context, _nowService, _settings);
        var tom = context.Employees!.Single();
        var mensCut30Min = context.Services!.Single();

        // Act
        var slots = await _sut.GetAvailableSlotsForEmployee(mensCut30Min.Id, tom.Id);

        // Assert
        var times = slots.DaysSlots.SelectMany(x => x.Times).ToArray();
        Assert.Equal(totalSlots, times.Length);
        for (int i = 0; i < expectedTimes.Length; i++)
        {
            var time = times[i];
            Assert.Equal(DateTime.Parse(expectedTimes[i]), time);
        }
    }

}