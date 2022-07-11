using Microsoft.AspNetCore.Mvc;
using Uqs.AppointmentBooking.Domain.Services;

namespace Uqs.AppointmentBooking.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SlotsController : ControllerBase
{
    private readonly ISlotsService _slotsService;

    public SlotsController(ISlotsService slotsService)
    {
        _slotsService = slotsService;
    }


    [HttpGet]
    public async Task<ActionResult<Contract.AvailableSlots>> GetSlots(string serviceId, string? employeeId)
    {
        var slots = await _slotsService.GetAvailableSlotsForEmployee(serviceId, employeeId);
       
        return null!;
    }
}
