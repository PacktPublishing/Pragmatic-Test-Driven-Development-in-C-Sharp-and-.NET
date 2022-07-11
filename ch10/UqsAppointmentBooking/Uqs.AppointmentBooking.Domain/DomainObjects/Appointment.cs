namespace Uqs.AppointmentBooking.Domain.DomainObjects;

public class Appointment : IEntity
{ 
    public string? Id { get; set; }
    public DateTime Starting { get; set; }
    public DateTime Ending { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int ServiceId { get; set; }
    public Service? Service { get; set; }
}
