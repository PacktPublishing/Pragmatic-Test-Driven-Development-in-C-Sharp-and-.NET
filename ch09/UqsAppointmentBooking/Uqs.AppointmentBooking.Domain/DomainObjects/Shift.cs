namespace Uqs.AppointmentBooking.Domain.DomainObjects;

public class Shift
{
    public int Id { get; set; }
    public DateTimeOffset Starting { get; set; }
    public DateTimeOffset Ending { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}