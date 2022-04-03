using Microsoft.EntityFrameworkCore;
using Uqs.AppointmentBooking.Database.Domain;
using Uqs.AppointmentBooking.Domain;
using Uqs.AppointmentBooking.Domain.Database;
using Uqs.AppointmentBooking.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppointmentBooking")));

// Add services to the container.
builder.Services.AddScoped<INowService, NowService>();
builder.Services.AddScoped<IServicesService, ServicesService>();
builder.Services.AddScoped<IEmployeesService, EmployeesService>();
builder.Services.AddScoped<ISlotsService, SlotsService>();
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection(nameof(ApplicationSettings)));
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await SeedData.Initialize(services);
    }
}

// Configure the HTTP request pipeline.

// Enable CORS
app.UseCors(x => x
   .AllowAnyMethod()
   .AllowAnyHeader()
   .SetIsOriginAllowed(origin => true) // allow any origin  
   .AllowCredentials());               // allow credentials 

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();