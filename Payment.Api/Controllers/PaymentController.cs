using Microsoft.AspNetCore.Mvc;
using Payment.Api.Domain;
using Payment.Api.Services;

namespace Payment.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentController : ControllerBase
{

    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(ILogger<PaymentController> logger,IPaymentService paymentService)
    {
        _logger = logger;
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> Pay([FromBody] PaymentRequestModel requestModel)
    {
        var response= await _paymentService.Pay(requestModel);
        return Ok(response);
    }
}