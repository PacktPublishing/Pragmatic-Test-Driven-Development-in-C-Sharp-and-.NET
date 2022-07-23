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
        var activeServices = await _servicesService.GetActiveServices();
        Contract.Service[] services = activeServices.Select(
            x => new Contract.Service(x.Id, x.Name!, x.AppointmentTimeSpanInMin, x.Price)).ToArray();
        return new Contract.AvailableServices(services);
    }

    [HttpGet("{serviceId}")]
    public async Task<ActionResult<Contract.Service>> GetService(string serviceId)
    {
        var service = await _servicesService.GetService(serviceId);
        if (service is null)
        {
            return NotFound();
        }
        Contract.Service serviceContract = new (
            service.Id, service.Name!, service.AppointmentTimeSpanInMin, service.Price);
        return serviceContract;
    }
}
