using Finance.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class FinanceController : ControllerBase
{

    private readonly IFinanceService _financeService;
    private readonly ILogger<FinanceController> _logger;

    public FinanceController(ILogger<FinanceController> logger,IFinanceService financeService)
    {
        _logger = logger;
        _financeService = financeService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int accountId)
    {
        var response = await _financeService.GetTotalBalance(accountId);
        return Ok(response);
    }
}