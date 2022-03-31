namespace Uqs.AppointmentBooking.Domain.DomainObjects;

public class Service
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public short AppointmentTimeSpanInMin { get; set; }
    public float Price { get; set; }
}
