namespace Uqs.AppointmentBooking.Domain.DomainObjects;

public class Appointment
{
    public int Id { get; set; }
    public DateTime Starting { get; set; }
    public DateTime Ending { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int ServiceId { get; set; }
    public Service? Service { get; set; }
}
