using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Uqs.AppointmentBooking.Domain.DomainObjects;
using Uqs.AppointmentBooking.Domain.Repository;

namespace Uqs.AppointmentBooking.Domain.Database;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var settings = serviceProvider.GetRequiredService<IOptions<ApplicationSettings>>();
        var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
        var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(settings.Value.DatabaseId);
        await databaseResponse.Database.CreateContainerIfNotExistsAsync(nameof(Customer), "/id");

        var customerRepository = serviceProvider.GetRequiredService<ICustomerRepository>();
        var customers = await customerRepository.GetAll();
        
        if (customers.Any())
        {
            return;   // DB has been seeded
        }

        await databaseResponse.Database.CreateContainerIfNotExistsAsync(nameof(Employee), "/id");
        await databaseResponse.Database.CreateContainerIfNotExistsAsync(nameof(Service), "/id");
        await databaseResponse.Database.CreateContainerIfNotExistsAsync(nameof(Appointment), "/id");


        var employeeRepository = serviceProvider.GetRequiredService<IEmployeeRepository>();

        // Employees
        var tom = new Employee { Id = Guid.NewGuid().ToString(), Name = "Thomas Fringe" };
        var tomShifts = new List<Shift>();
        var jane = new Employee { Id = Guid.NewGuid().ToString(), Name = "Jane Haircomb" };
        var janeShifts = new List<Shift>();
        var will = new Employee { Id = Guid.NewGuid().ToString(), Name = "William Scissors" };
        var willShifts = new List<Shift>();
        var jess = new Employee { Id = Guid.NewGuid().ToString(), Name = "Jessica Clipper" };
        var jessShifts = new List<Shift>();
        var ed = new Employee { Id = Guid.NewGuid().ToString(), Name = "Edward Sideburn" };
        var edShifts = new List<Shift>();
        var oli = new Employee { Id = Guid.NewGuid().ToString(), Name = "Oliver Bold" };
        var oliShifts = new List<Shift>();

        // Shifts
        var now = DateTime.Now;
        for (var i = 0;i < 10;i++)
        {
            DateTime date = now.AddDays(i);

            // Sunday the salon is closed
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                continue;
            }

            // Thomas works all shifts except Saturdays
            if (date.DayOfWeek != DayOfWeek.Saturday)
            {
                tomShifts.AddRange(new[] { 
                    new Shift { Starting = SetTime(date, "09:00"), Ending = SetTime(date, "12:00") },
                    new Shift { Starting = SetTime(date, "13:00"), Ending = SetTime(date, "18:00") }
                });
            }

            // Jane works at peak times only
            janeShifts.Add(new Shift { Starting = SetTime(date, "11:00"), Ending = SetTime(date, "14:00") });
            
            // Will works Friday and Saturday only
            if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
            {
                willShifts.AddRange(new[] {
                    new Shift { Starting = SetTime(date, "09:00"), Ending = SetTime(date, "12:00") },
                    new Shift { Starting = SetTime(date, "13:00"), Ending = SetTime(date, "18:00") }
                });
            }

            // Jess works all week but takes a longer lunch break
            jessShifts.AddRange(new[] {
                    new Shift { Starting = SetTime(date, "09:00"), Ending = SetTime(date, "11:30") },
                    new Shift { Starting = SetTime(date, "14:30"), Ending = SetTime(date, "18:00") }
                });

            // Ed works morning shifts
            edShifts.Add(new Shift { Starting = SetTime(date, "09:00"), Ending = SetTime(date, "12:00") });

            // Oli works afternoon shifts
            oliShifts.Add(new Shift { Starting = SetTime(date, "13:00"), Ending = SetTime(date, "18:00") });
        }

        tom.Shifts = tomShifts.ToArray();
        await employeeRepository.AddItemAsync(tom);
        jane.Shifts = janeShifts.ToArray();
        await employeeRepository.AddItemAsync(jane);
        will.Shifts = willShifts.ToArray();
        await employeeRepository.AddItemAsync(will);
        jess.Shifts = jessShifts.ToArray();
        await employeeRepository.AddItemAsync(jess);
        ed.Shifts = edShifts.ToArray();
        await employeeRepository.AddItemAsync(ed);
        oli.Shifts = oliShifts.ToArray();
        await employeeRepository.AddItemAsync(oli);

        // Customers
        var paul = new Customer { Id = Guid.NewGuid().ToString(), FirstName = "Paul", LastName = "Longhair" };
        await customerRepository.AddItemAsync(paul);
        var harry = new Customer { Id = Guid.NewGuid().ToString(), FirstName = "Harry", LastName = "Gingerman" };
        await customerRepository.AddItemAsync(harry);
        var john = new Customer { Id = Guid.NewGuid().ToString(), FirstName = "Johny", LastName = "Undercut" };
        await customerRepository.AddItemAsync(john);
        var ian = new Customer { Id = Guid.NewGuid().ToString(), FirstName = "Ian", LastName = "Taperfade" };
        await customerRepository.AddItemAsync(ian);

        // Services
        var serviceRepository = serviceProvider.GetRequiredService<IServiceRepository>();
        
        var mensCut = new Service 
        { Id = Guid.NewGuid().ToString(), Name = "Men's Cut", AppointmentTimeSpanInMin = 30, Price = 23, IsActive = true };
        await serviceRepository.AddItemAsync(mensCut);
        var mensClipperScissor = new Service
        { Id = Guid.NewGuid().ToString(), Name = "Men - Clipper & Scissor Cut", AppointmentTimeSpanInMin = 30, Price = 23, IsActive = true };
        await serviceRepository.AddItemAsync(mensClipperScissor);
        var mensBeardTrim = new Service
        { Id = Guid.NewGuid().ToString(), Name = "Men - Beard Trim", AppointmentTimeSpanInMin = 10, Price = 10, IsActive = true };
        await serviceRepository.AddItemAsync(mensBeardTrim);
        var mensColoring = new Service
        { Id = Guid.NewGuid().ToString(), Name = "Men - Full Head Coloring", AppointmentTimeSpanInMin = 70, Price = 60, IsActive = true };
        await serviceRepository.AddItemAsync(mensColoring);
        var mensPerm = new Service
        { Id = Guid.NewGuid().ToString(), Name = "Men - Perm", AppointmentTimeSpanInMin = 100, Price = 90, IsActive = true };
        await serviceRepository.AddItemAsync(mensPerm);
        var mensKeratin = new Service
        { Id = Guid.NewGuid().ToString(), Name = "Men - Keratin Treatment", AppointmentTimeSpanInMin = 120, Price = 100, IsActive = false };
        await serviceRepository.AddItemAsync(mensKeratin);
        var boysCut = new Service
        { Id = Guid.NewGuid().ToString(), Name = "Boys - Cut", AppointmentTimeSpanInMin = 30, Price = 15, IsActive = true };
        await serviceRepository.AddItemAsync(boysCut);
        var girlsCut = new Service
        { Id = Guid.NewGuid().ToString(), Name = "Girls - Cut", AppointmentTimeSpanInMin = 30, Price = 17, IsActive = true };
        await serviceRepository.AddItemAsync(girlsCut);

        // Appointments
        var appointmentRepository = serviceProvider.GetRequiredService<IAppointmentRepository>();

        var appointment1 = new Appointment
        { Id = Guid.NewGuid().ToString(), CustomerId = paul.Id, EmployeeId = tom.Id, ServiceId = mensCut.Id, Starting = GetFirstMonday() };
        await appointmentRepository.AddItemAsync(appointment1);
    }

    private static DateTime GetFirstMonday()
    {
        var now = SetTime(DateTime.Now.Date, "9:30");
        var diff = DayOfWeek.Monday - now.DayOfWeek;
        var monday = now.AddDays(diff);

        return monday;
    }

    private static DateTime SetTime(DateTime date, string time)
    {
        var hourMin = time.Split(":").Select(byte.Parse).ToArray();
        return new DateTime(date.Year, date.Month, date.Day, hourMin[0], hourMin[1], 0);
    }
}