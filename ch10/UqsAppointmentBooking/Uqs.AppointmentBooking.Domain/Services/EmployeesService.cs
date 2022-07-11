using Microsoft.Azure.Cosmos;
using Uqs.AppointmentBooking.Domain.DomainObjects;
using Uqs.AppointmentBooking.Domain.Repository;

namespace Uqs.AppointmentBooking.Domain.Services;

public interface IEmployeesService
{
    Task<IEnumerable<Employee>> GetEmployees();
}

public class EmployeesService : IEmployeesService
{
    private readonly IEmployeeRepository employeesRepository;

    public EmployeesService(IEmployeeRepository employeesRepository)
    {
        this.employeesRepository = employeesRepository;
    }

    public async Task<IEnumerable<Employee>> GetEmployees()
        => await employeesRepository.GetAllAsync();
}