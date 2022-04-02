namespace Uqs.AppointmentBooking.Domain.DomainObjects;

public class Shift
{
    public int Id { get; set; }
    public DateTime Starting { get; set; }
    public DateTime Ending { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}