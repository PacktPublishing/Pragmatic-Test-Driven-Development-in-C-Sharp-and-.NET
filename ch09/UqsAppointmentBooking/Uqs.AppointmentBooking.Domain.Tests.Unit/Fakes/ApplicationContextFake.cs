using Microsoft.EntityFrameworkCore;
using Uqs.AppointmentBooking.Database.Domain;

namespace Uqs.AppointmentBooking.Domain.Tests.Unit.Fakes;

public class ApplicationContextFake : ApplicationContext
{
    private static DbContextOptions<ApplicationContext> _efOptions = 
        new DbContextOptionsBuilder<ApplicationContext>()
        .UseInMemoryDatabase(databaseName: "AppointmentBookingTest")
        .Options;

    public ApplicationContextFake() : base(_efOptions) {}
}
