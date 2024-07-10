using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsService
{
    private readonly ILogger<PaymentsService> _logger;
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IBankAuthorizationClient _bankAuthorizationClient;

    public PaymentsService(ILogger<PaymentsService> logger, IPaymentsRepository paymentsRepository, IBankAuthorizationClient bankAuthorizationClient)
    {
        _logger = logger;
        _paymentsRepository = paymentsRepository;
        _bankAuthorizationClient = bankAuthorizationClient;
    }

    public async Task<PostPaymentResponse> ProcessPaymentAsync(PostPaymentRequest request)
    {
        var paymentId = Guid.NewGuid();
        _logger.LogInformation("Processing new payment request: {@id}", paymentId);

        // Authorize payment via bank.
        var paymentStatus = await _bankAuthorizationClient.AuthorizationRequest(request);
        _logger.LogInformation("Payment with id {@id} status: {@authorized}", paymentId, paymentStatus);

        // Store payment.
        var payment = new PostPaymentResponse(Guid.NewGuid(), request, paymentStatus);
        await _paymentsRepository.Add(payment);
        _logger.LogInformation("Payment with id {@id} stored", paymentId);
        return payment;
    }

    public async Task<PostPaymentResponse?> GetPaymentAsync(Guid id)
    {
        _logger.LogInformation("Get payment with id {@id}", id);
        var payment = await _paymentsRepository.Get(id);
        return payment;
    }
}
