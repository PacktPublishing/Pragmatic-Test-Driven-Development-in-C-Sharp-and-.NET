using Microsoft.EntityFrameworkCore;
using Uqs.AppointmentBooking.Database.Domain;
using Uqs.AppointmentBooking.Domain.DomainObjects;
using Uqs.AppointmentBooking.Domain.Report;

namespace Uqs.AppointmentBooking.Domain.Services;

public interface ISlotService
{
    Task<Slots> GetAvailableSlotsForEmployee(int serviceId, int employeeId);
}

public class SlotsService : ISlotService
{
    private readonly ApplicationContext _context;
    private readonly DateTime _now;
    internal const byte DAYS = 7;
    internal const byte APPOINTMENT_INCREMENT_MIN = 5;
    private static TimeSpan _roundingIntervalSpan = TimeSpan.FromMinutes(APPOINTMENT_INCREMENT_MIN);

    public SlotsService(ApplicationContext context, INowService nowService)
    {
        _context = context;
        _now = GetRoundedToNearestInterval(nowService.Now);
    }

    public async Task<Slots> GetAvailableSlotsForEmployee(int serviceId, int employeeId)
    {
        Service service = await _context.Services!.SingleAsync(x => x.Id == serviceId);
        DateTime openAppointmentsEnd = GetEndOfOpenAppointments();

        List<(DateTime From, DateTime To)> timeIntervals = new();

        var shifts = _context.Shifts!.Where(
            x => x.EmployeeId == employeeId && 
            x.Ending < openAppointmentsEnd &&
            (x.Starting <= _now && x.Ending > _now || x.Starting > _now));
        
        foreach(var shift in shifts)
        {
            DateTime potentialAppointmentStart = shift.Starting;
            DateTime potentialAppointmentEnd = 
                potentialAppointmentStart.AddMinutes(service.AppointmentTimeSpanInMin);
            
            for(int increment = 0;potentialAppointmentEnd <= shift.Ending;increment += APPOINTMENT_INCREMENT_MIN)
            {
                potentialAppointmentStart = potentialAppointmentStart.AddMinutes(increment);
                potentialAppointmentEnd = potentialAppointmentStart.AddMinutes(service.AppointmentTimeSpanInMin);
                if (potentialAppointmentEnd <= shift.Ending)
                {
                    timeIntervals.Add((potentialAppointmentStart, potentialAppointmentEnd));
                }
            }
        }

        IEnumerable<DateTime> uniqueDays = timeIntervals
            .DistinctBy(x => x.From.Date)
            .Select(x => x.From.Date);

        List<DaySlots> daySlotsList = new List<DaySlots>();

        foreach(var day in uniqueDays)
        {
            var startTimes = timeIntervals.Where(x => x.From.Date == day.Date).Select(x => x.From).ToArray();
            var daySlots = new DaySlots(day, startTimes);
            daySlotsList.Add(daySlots);
        }

        var slots = new Slots(daySlotsList.ToArray());

        return slots;
    }

    private DateTime GetEndOfOpenAppointments()
        => _now.Date.AddDays(DAYS);

    private DateTime GetRoundedToNearestInterval(DateTime dt)
    {
        long ticksInSpan = _roundingIntervalSpan.Ticks;
        return new DateTime(
            (dt.Ticks + ticksInSpan - 1) 
            / ticksInSpan * ticksInSpan, dt.Kind);
    }
}