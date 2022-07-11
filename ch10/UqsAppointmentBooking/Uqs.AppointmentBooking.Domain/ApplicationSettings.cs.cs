namespace Uqs.AppointmentBooking.Domain;

public class ApplicationSettings
{
    public string? DatabaseId { get; set; }
    public byte OpenAppointmentInDays { get; set; }
    public byte RoundUpInMin { get; set; }
    public byte RestInMin { get; set; }
}
