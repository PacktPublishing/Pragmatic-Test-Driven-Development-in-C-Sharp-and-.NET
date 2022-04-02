namespace Uqs.AppointmentBooking.Domain.Report;

public record DaySlots(DateTimeOffset Day, DateTimeOffset[] Times);