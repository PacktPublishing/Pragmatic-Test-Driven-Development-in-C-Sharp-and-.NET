using Microsoft.AspNetCore.Mvc;
using Uqs.AppointmentBooking.Domain.Services;

namespace Uqs.AppointmentBooking.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeesService _employeesService;

    public EmployeesController(IEmployeesService employeesService)
    {
        _employeesService = employeesService;
    }

    [HttpGet]
    public async Task<ActionResult<Contract.AvailableEmployees>> AvailableEmployees()
    {
        var employees = await _employeesService.GetEmployees();
        Contract.Employee[] emps = employees.Select(
            x => new Contract.Employee(x.Id, x.Name!)).ToArray();
        return new Contract.AvailableEmployees(emps);
    }

}
