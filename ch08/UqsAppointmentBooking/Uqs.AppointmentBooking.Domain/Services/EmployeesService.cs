using Uqs.AppointmentBooking.Domain.DomainObjects;

namespace Uqs.AppointmentBooking.Domain.Services;

public interface IEmployeesService
{
    Task<IEnumerable<Employee>> GetEmployees();
}

public class EmployeesService : IEmployeesService
{
    public async Task<IEnumerable<Employee>> GetEmployees()
        => throw new NotImplementedException();
}