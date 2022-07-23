namespace Uqs.AppointmentBooking.Domain.DomainObjects;

public class Customer : IEntity
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
