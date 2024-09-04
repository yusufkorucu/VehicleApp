using Microsoft.AspNetCore.Mvc;
using Vehicle.Api.Models;
using Vehicle.Api.Services;

namespace Vehicle.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<VehicleController> _logger;

    public VehicleController(ILogger<VehicleController> logger, IVehicleService vehicleService)
    {
        _logger = logger;
        _vehicleService = vehicleService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PlateQueryRequestModel requestModel)
    {
        var response = await _vehicleService.GetVehicleByPlateAsync(requestModel);

        if (response.IsSuccess)
            return Ok(response);

        return BadRequest(response);
    }
}