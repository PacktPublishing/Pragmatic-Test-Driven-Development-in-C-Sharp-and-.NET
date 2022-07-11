using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Uqs.AppointmentBooking.Domain.DomainObjects;
using Uqs.AppointmentBooking.Domain.Repository;

namespace Uqs.AppointmentBooking.Domain.Database;

public static class SeedData
{
    public async static Task Initialize(IServiceProvider serviceProvider)
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

        var employeeRepository = serviceProvider.GetRequiredService<IEmployeeRepository>();

        // Employees
        var tom = new Employee { Id = Guid.NewGuid().ToString(), Name = "Thomas Fringe" };
        await employeeRepository.AddItemAsync(tom);
        var jane = new Employee { Id = Guid.NewGuid().ToString(), Name = "Jane Haircomb" };
        await employeeRepository.AddItemAsync(jane);
        var will = new Employee { Id = Guid.NewGuid().ToString(), Name = "William Scissors" };
        await employeeRepository.AddItemAsync(will);
        var jess = new Employee { Id = Guid.NewGuid().ToString(), Name = "Jessica Clipper" };
        await employeeRepository.AddItemAsync(jess);
        var ed = new Employee { Id = Guid.NewGuid().ToString(), Name = "Edward Sideburn" };
        await employeeRepository.AddItemAsync(ed);
        var oli = new Employee { Id = Guid.NewGuid().ToString(), Name = "Oliver Bold" };
        await employeeRepository.AddItemAsync(oli);

        // Shifts
        DateTime now = DateTime.Now;
        for (int i = 0;i < 10;i++)
        {
            DateTime date = now.AddDays(i);

            // Sunday the salon is closed
            if (date.DayOfWeek != DayOfWeek.Sunday)
            {
                continue;
            }

            // Thomas works all shifts except Saturdays
            if (date.DayOfWeek != DayOfWeek.Saturday)
            {
                tom.Shifts = new[]{ 
                    new Shift { Starting = SetTime(date, "09:00"), Ending = SetTime(date, "12:00") },
                    new Shift { Starting = SetTime(date, "13:00"), Ending = SetTime(date, "18:00") }
                };
            }
            await employeeRepository.UpdateItemAsync(tom.Id, tom);

            // Jane works at peak times only
            jane.Shifts = new[] { 
                new Shift { Starting = SetTime(date, "11:00"), Ending = SetTime(date, "14:00") } 
            };
            await employeeRepository.UpdateItemAsync(jane.Id, jane);

            // Will works Friday and Saturday only
            if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
            {
                will.Shifts = new[]{
                    new Shift { Starting = SetTime(date, "09:00"), Ending = SetTime(date, "12:00") },
                    new Shift { Starting = SetTime(date, "13:00"), Ending = SetTime(date, "18:00") }
                };
                await employeeRepository.UpdateItemAsync(will.Id, will);
            }

            // Jess works all week but takes a longer lunch break
            jess.Shifts = new[]{
                    new Shift { Starting = SetTime(date, "09:00"), Ending = SetTime(date, "11:30") },
                    new Shift { Starting = SetTime(date, "14:30"), Ending = SetTime(date, "18:00") }
                };
            await employeeRepository.UpdateItemAsync(jess.Id, jess);

            // Ed works morning shifts
            ed.Shifts = new[] {
                new Shift { Starting = SetTime(date, "09:00"), Ending = SetTime(date, "12:00") }
            };
            await employeeRepository.UpdateItemAsync(ed.Id, ed);

            // Oli works afternoon shifts
            oli.Shifts = new[] {
                new Shift { Starting = SetTime(date, "13:00"), Ending = SetTime(date, "18:00") }
            };
            await employeeRepository.UpdateItemAsync(oli.Id, oli);
        }

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
    }


    private static DateTime SetTime(DateTime date, string time)
    {
        byte[] hourMin = time.Split(":").Select(x => byte.Parse(x)).ToArray();
        return new DateTime(date.Year, date.Month, date.Day, hourMin[0], hourMin[1], 0);
    }
}