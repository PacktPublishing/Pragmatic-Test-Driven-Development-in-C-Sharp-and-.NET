using Microsoft.Extensions.Options;
using Uqs.AppointmentBooking.Domain.Report;
using Uqs.AppointmentBooking.Domain.Repository;

namespace Uqs.AppointmentBooking.Domain.Services;

public interface ISlotsService
{
    Task<Slots> GetAvailableSlotsForEmployee(string serviceId, string employeeId);
}

public class SlotsService : ISlotsService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ApplicationSettings _settings;
    private readonly DateTime _now;
    private readonly TimeSpan _roundingIntervalSpan;

    public SlotsService(IServiceRepository serviceRepository,
        IEmployeeRepository employeeRepository,
        IAppointmentRepository appointmentRepository,
        INowService nowService, IOptions<ApplicationSettings> settings)
    {
        _serviceRepository = serviceRepository;
        _employeeRepository = employeeRepository;
        _appointmentRepository = appointmentRepository;
        _settings = settings.Value;
        _roundingIntervalSpan = TimeSpan.FromMinutes(_settings.RoundUpInMin);
        _now = RoundUpToNearest(nowService.Now);
    }

    public async Task<Slots> GetAvailableSlotsForEmployee(string serviceId, string employeeId)
    {
        var service = await _serviceRepository.GetItemAsync(serviceId);
        if (service is null)
        {
            throw new ArgumentException("Record not found", nameof(serviceId));
        }
        var employee = await _employeeRepository.GetItemAsync(employeeId);
        if (employee is null)
        {
            throw new ArgumentException("Record not found", nameof(employeeId));
        }
        var appointmentsMaxDay = GetEndOfOpenAppointments();

        List<(DateTime From, DateTime To)> timeIntervals = new();
   
        var shifts = employee.Shifts!.Where(x => 
            x.Ending < appointmentsMaxDay &&
            ((x.Starting <= _now && x.Ending > _now) || x.Starting > _now));
        
        foreach(var shift in shifts)
        {
            var potentialAppointmentStart = shift.Starting;
            var potentialAppointmentEnd = 
                potentialAppointmentStart.AddMinutes(service.AppointmentTimeSpanInMin);
            
            for(int increment = 0;potentialAppointmentEnd <= shift.Ending;increment += _settings.RoundUpInMin)
            {
                potentialAppointmentStart = shift.Starting.AddMinutes(increment);
                potentialAppointmentEnd = potentialAppointmentStart.AddMinutes(service.AppointmentTimeSpanInMin);
                if (potentialAppointmentEnd <= shift.Ending)
                {
                    timeIntervals.Add((potentialAppointmentStart, potentialAppointmentEnd));
                }
            }
        }
        var employeeAppointments = await _appointmentRepository.GetAppointmentsByEmployeeIdAsync(employeeId);
        var appointments = employeeAppointments.Where(x =>
            x.Ending < appointmentsMaxDay &&
            ((x.Starting <= _now && x.Ending > _now) && x.Starting > _now)).ToArray();

        foreach(var appointment in appointments)
        {
            DateTime appointmentStartWithRest = appointment.Starting.AddMinutes(-_settings.RestInMin);
            DateTime appointmentEndWithRest = appointment.Ending.AddMinutes(_settings.RestInMin);
            timeIntervals.RemoveAll(x =>
                IsPeriodIntersecting(x.From, x.To, appointmentStartWithRest, appointmentEndWithRest));
        }

        var uniqueDays = timeIntervals
            .DistinctBy(x => x.From.Date)
            .Select(x => x.From.Date);

        var daySlotsList = new List<DaySlots>();

        foreach(var day in uniqueDays)
        {
            var startTimes = timeIntervals.Where(x => x.From.Date == day.Date).Select(x => x.From).ToArray();
            var daySlots = new DaySlots(day, startTimes);
            daySlotsList.Add(daySlots);
        }

        var slots = new Slots(daySlotsList.ToArray());

        return slots;
    }

    private DateTime GetEndOfOpenAppointments() => _now.Date.AddDays(
        _settings.OpenAppointmentInDays);

    private DateTime RoundUpToNearest(DateTime dt)
    {
        var ticksInSpan = _roundingIntervalSpan.Ticks;
        return new DateTime((dt.Ticks + ticksInSpan - 1) 
            / ticksInSpan * ticksInSpan, dt.Kind);
    }

    private bool IsPeriodIntersecting(DateTime fromT1, DateTime toT1, DateTime fromT2, DateTime toT2) 
        => fromT1 < toT2 && toT1 > fromT2;
}