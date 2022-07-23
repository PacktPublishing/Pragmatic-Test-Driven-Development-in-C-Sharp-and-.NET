namespace Uqs.AppointmentBooking.Domain.DomainObjects;

public class Employee : IEntity
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public Shift[]? Shifts { get; set; }
}
