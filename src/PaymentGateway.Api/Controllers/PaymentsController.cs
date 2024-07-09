using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly PaymentsService _paymentsService;

    public PaymentsController(PaymentsService paymentsService)
    {
        _paymentsService = paymentsService;
    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> PostPaymentAsync(PostPaymentRequest request)
    {
        var payment = await _paymentsService.ProcessPaymentAsync(request);
        return new CreatedResult($"/api/Payments/{payment.Id}", payment);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = await _paymentsService.GetPaymentAsync(id);
        if (payment == null)
        {
            return NotFound();
        }

        return new OkObjectResult(payment);
    }
}