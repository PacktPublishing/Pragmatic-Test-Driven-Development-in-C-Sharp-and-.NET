namespace Uqs.AppointmentBooking.Domain.DomainObjects;

public class Appointment : IEntity
{ 
    public string? Id { get; set; }
    public DateTime Starting { get; set; }
    public DateTime Ending { get; set; }
    public string? CustomerId { get; set; }
    public string? EmployeeId { get; set; }
    public string? ServiceId { get; set; }
}
