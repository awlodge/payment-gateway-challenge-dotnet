using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly ILogger<PaymentsController> _logger;
    private readonly PaymentsService _paymentsService;

    public PaymentsController(ILogger<PaymentsController> logger, PaymentsService paymentsService)
    {
        _logger = logger;
        _paymentsService = paymentsService;
    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> PostPaymentAsync(PostPaymentRequest request)
    {
        _logger.LogInformation("Processing POST payment request");
        var payment = await _paymentsService.ProcessPaymentAsync(request);
        _logger.LogInformation("Payment created: {@id}", payment.Id);
        return new CreatedResult($"/api/Payments/{payment.Id}", payment);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        _logger.LogInformation("Processing GET payment request: {@id}", id);
        var payment = await _paymentsService.GetPaymentAsync(id);
        if (payment == null)
        {
            _logger.LogInformation("Payment not found: {@id}", id);
            return NotFound();
        }

        _logger.LogInformation("Payment found: {@id}", id);
        return new OkObjectResult(payment);
    }
}