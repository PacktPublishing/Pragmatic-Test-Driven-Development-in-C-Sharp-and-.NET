using Microsoft.AspNetCore.Mvc;
using Uqs.AppointmentBooking.Domain.Services;

namespace Uqs.AppointmentBooking.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServicesService _servicesService;

    public ServicesController(IServicesService servicesService)
    {
        _servicesService = servicesService;
    }

    [HttpGet]
    public async Task<ActionResult<Contract.AvailableServices>> AvailableServices()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{serviceId:int}")]
    public async Task<ActionResult<Contract.Service>> GetService(int serviceId)
    {
        throw new NotImplementedException();
    }
}
