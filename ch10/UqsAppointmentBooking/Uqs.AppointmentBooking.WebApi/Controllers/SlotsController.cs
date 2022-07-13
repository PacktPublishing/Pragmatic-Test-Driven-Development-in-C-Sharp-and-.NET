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


    [HttpGet("{serviceId}")]
    public async Task<ActionResult<Contract.AvailableSlots>> Slots(string serviceId, [FromQuery] string employeeId)
    {
        var slots = await _slotsService.GetAvailableSlotsForEmployee(serviceId, employeeId);

        var daySlots = slots.DaysSlots.Select(x => new Contract.DaySlots(x.Day, x.Times)).ToArray();

        return new Contract.AvailableSlots(daySlots);
    }
}
