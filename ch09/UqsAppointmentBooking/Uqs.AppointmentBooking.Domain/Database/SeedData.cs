using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Uqs.AppointmentBooking.Domain.DomainObjects;

namespace Uqs.AppointmentBooking.Domain.Database;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationContext>>());
        await context.Database.EnsureCreatedAsync();
        
        if (await context.Customers!.AnyAsync())
        {
            return;   // DB has been seeded
        }

        // Employees
        var tom = await context.AddAsync(new Employee { Name = "Thomas Fringe" });
        var jane = await context.AddAsync(new Employee { Name = "Jane Haircomb" });
        var will = await context.AddAsync(new Employee { Name = "William Scissors" });
        var jess = await context.AddAsync(new Employee { Name = "Jessica Clipper" });
        var ed = await context.AddAsync(new Employee { Name = "Edward Sideburn" });
        var oli = await context.AddAsync(new Employee { Name = "Oliver Bold" });

        // Shifts
        DateTime now = DateTime.Now;
        for (int i = 0;i < 10;i++)
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
                await context.AddAsync(new Shift { Employee = tom.Entity, Starting = SetTime(date, "09:00"), Ending = SetTime(date, "12:00") });
                await context.AddAsync(new Shift { Employee = tom.Entity, Starting = SetTime(date, "13:00"), Ending = SetTime(date, "18:00") });
            }

            // Jane works at peak times only   
            await context.AddAsync(new Shift { Employee = jane.Entity, Starting = SetTime(date, "11:00"), Ending = SetTime(date, "14:00") });

            // Will works Friday and Saturday only
            if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
            {
                await context.AddAsync(new Shift { Employee = will.Entity, Starting = SetTime(date, "09:00"), Ending = SetTime(date, "12:00") });
                await context.AddAsync(new Shift { Employee = will.Entity, Starting = SetTime(date, "13:00"), Ending = SetTime(date, "18:00") });
            }

            // Jess works all week but takes a longer lunch break
            await context.AddAsync(new Shift { Employee = jess.Entity, Starting = SetTime(date, "09:00"), Ending = SetTime(date, "11:30") });
            await context.AddAsync(new Shift { Employee = jess.Entity, Starting = SetTime(date, "14:30"), Ending = SetTime(date, "18:00") });

            // Ed works morning shifts
            await context.AddAsync(new Shift { Employee = ed.Entity, Starting = SetTime(date, "09:00"), Ending = SetTime(date, "12:00") });

            // Oli works afternoon shifts
            await context.AddAsync(new Shift { Employee = oli.Entity, Starting = SetTime(date, "13:00"), Ending = SetTime(date, "18:00") });
        }

        // Customers
        var paul = await context.AddAsync(new Customer { FirstName = "Paul", LastName = "Longhair" });
        var harry = await context.AddAsync(new Customer { FirstName = "Harry", LastName = "Gingerman" });
        var john = await context.AddAsync(new Customer { FirstName = "Johny", LastName = "Undercut" });
        var ian = await context.AddAsync(new Customer { FirstName = "Ian", LastName = "Taperfade" });

        // Services
        var mensCut = await context.AddAsync(new Service 
        { Name = "Men's Cut", AppointmentTimeSpanInMin = 30, Price = 23, IsActive = true });
        var mensClipperScissor = await context.AddAsync(new Service
        { Name = "Men - Clipper & Scissor Cut", AppointmentTimeSpanInMin = 30, Price = 23, IsActive = true });
        var mensBeardTrim = await context.AddAsync(new Service
        { Name = "Men - Beard Trim", AppointmentTimeSpanInMin = 10, Price = 10, IsActive = true });
        var mensColoring = await context.AddAsync(new Service
        { Name = "Men - Full Head Coloring", AppointmentTimeSpanInMin = 70, Price = 60, IsActive = true });
        var mensPerm = await context.AddAsync(new Service
        { Name = "Men - Perm", AppointmentTimeSpanInMin = 100, Price = 90, IsActive = true });
        var mensKeratin = await context.AddAsync(new Service
        { Name = "Men - Keratin Treatment", AppointmentTimeSpanInMin = 120, Price = 100, IsActive = false });
        var boysCut = await context.AddAsync(new Service
        { Name = "Boys - Cut", AppointmentTimeSpanInMin = 30, Price = 15, IsActive = true });
        var girlsCut = await context.AddAsync(new Service
        { Name = "Girls - Cut", AppointmentTimeSpanInMin = 30, Price = 17, IsActive = true });

        context.SaveChanges();
    }

    private static DateTime SetTime(DateTime date, string time)
    {
        byte[] hourMin = time.Split(":").Select(x => byte.Parse(x)).ToArray();
        return new DateTime(date.Year, date.Month, date.Day, hourMin[0], hourMin[1], 0);
    }
}