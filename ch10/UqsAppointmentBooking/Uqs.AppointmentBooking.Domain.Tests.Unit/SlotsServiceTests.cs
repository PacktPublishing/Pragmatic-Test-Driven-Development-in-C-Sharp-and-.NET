using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uqs.AppointmentBooking.Domain.DomainObjects;
using Uqs.AppointmentBooking.Domain.Repository;
using Uqs.AppointmentBooking.Domain.Services;
using Xunit;

namespace Uqs.AppointmentBooking.Domain.Tests.Unit;

public class SlotsServiceTests
{
    private readonly IServiceRepository _serviceRepository = Substitute.For<IServiceRepository>();
    private readonly IEmployeeRepository _employeeRepository = Substitute.For<IEmployeeRepository>();
    private readonly IAppointmentRepository _appointmentRepository = Substitute.For<IAppointmentRepository>();
    private readonly INowService _nowService = Substitute.For<INowService>();
    private readonly ApplicationSettings _applicationSettings =
        new()
        {
            OpenAppointmentInDays = 7,
            RoundUpInMin = 5,
            RestInMin = 5
        };
    private readonly IOptions<ApplicationSettings> _settings =
        Substitute.For<IOptions<ApplicationSettings>>();
    private SlotsService _sut;

    public SlotsServiceTests()
    {
        _settings.Value.Returns(_applicationSettings);
        _sut = new SlotsService(_serviceRepository,
            _employeeRepository,
            _appointmentRepository,
            _nowService, _settings);
    }

    [Fact]
    public async Task GetAvailableSlotsForEmployee_ServiceIdNoFound_ArgumentException()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.GetAvailableSlotsForEmployee("SomeServiceId", "SomeEmpId"));

        // Assert
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public async Task GetAvailableSlotsForEmployee_EmployeeIdNoFound_ArgumentException()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.GetAvailableSlotsForEmployee("SomeServiceId", "SomeEmpId"));

        // Assert
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public async Task GetAvailableSlotsForEmployee_NoShiftsForTomAndNoAppointmentsInSystem_NoSlots()
    {
        // Arrange
        var appointmentFrom = new DateTime(2022, 10, 3, 7, 0, 0);
        _nowService.Now.Returns(appointmentFrom);
        var tom = new Employee { Id = "Tom", Name = "Thomas Fringe", Shifts = Array.Empty<Shift>() };
        var mensCut30Min = new Service { Id = "MensCut30Min", AppointmentTimeSpanInMin = 30 };
        _serviceRepository.GetItemAsync(Arg.Any<string>())
            .Returns(Task.FromResult((Service?)mensCut30Min));
        _employeeRepository.GetItemAsync(Arg.Any<string>())
            .Returns(Task.FromResult((Employee?)tom));

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
        var timeOfBooking = new DateTime(2022, 10, 3, 7, 0, 0);
        _nowService.Now.Returns(timeOfBooking);
        var shiftFrom = new DateTime(2022, 10, 3, 9, 0, 0);
        var shiftTo = shiftFrom.AddMinutes(serviceDuration);
        var tom = new Employee { Id = "Tom", Name = "Thomas Fringe", Shifts = 
            new [] { new Shift { Starting = shiftFrom, Ending = shiftTo } } };
        var mensCut30Min = new Service { Id = "MensCut30Min", AppointmentTimeSpanInMin = 30 };
        var tomAppointments = Array.Empty<Appointment>();

        _serviceRepository.GetItemAsync(Arg.Any<string>())
            .Returns(Task.FromResult((Service?)mensCut30Min));
        _employeeRepository.GetItemAsync(Arg.Any<string>())
            .Returns(Task.FromResult((Employee?)tom));
        _appointmentRepository.GetAppointmentsByEmployeeIdAsync(Arg.Is("Tom"))
            .Returns(Task.FromResult((IEnumerable<Appointment>)tomAppointments));

        _sut = new SlotsService(_serviceRepository, _employeeRepository, _appointmentRepository, 
            _nowService, _settings);

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
        var tom = new Employee { Id = "Tom", Name = "Thomas Fringe", Shifts = 
            new [] { new Shift { Starting = shiftFrom, Ending = shiftTo } } };
        var mensCut30Min = new Service { Id = "MensCut30Min", AppointmentTimeSpanInMin = 30 };
        var paul = new Customer { Id = "Paul", FirstName = "Paul", LastName = "Longhair" };
        var tomAppointments = new[] { new Appointment { CustomerId = paul.Id, 
            ServiceId = mensCut30Min.Id, Starting = appointmentStart , Ending = appointmentEnd } };

        _serviceRepository.GetItemAsync(Arg.Any<string>())
            .Returns(Task.FromResult((Service?)mensCut30Min));
        _employeeRepository.GetItemAsync(Arg.Any<string>())
            .Returns(Task.FromResult((Employee?)tom));
        _appointmentRepository.GetAppointmentsByEmployeeIdAsync(Arg.Is("Tom"))
            .Returns(Task.FromResult((IEnumerable<Appointment>)tomAppointments));

        _sut = new SlotsService(_serviceRepository, _employeeRepository, _appointmentRepository,
            _nowService, _settings);

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