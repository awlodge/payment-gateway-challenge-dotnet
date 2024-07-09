using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsService
{
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IBankAuthorizationClient _bankAuthorizationClient;

    public PaymentsService(IPaymentsRepository paymentsRepository, IBankAuthorizationClient bankAuthorizationClient)
    {
        _paymentsRepository = paymentsRepository;
        _bankAuthorizationClient = bankAuthorizationClient;
    }

    public async Task<PostPaymentResponse> ProcessPaymentAsync(PostPaymentRequest request)
    {
        // Authorize payment via bank.
        var paymentStatus = await _bankAuthorizationClient.AuthorizationRequest(request);

        // Store payment.
        var payment = new PostPaymentResponse(Guid.NewGuid(), request, paymentStatus);
        await _paymentsRepository.Add(payment);
        return payment;
    }

    public async Task<PostPaymentResponse?> GetPaymentAsync(Guid id)
    {
        var payment = await _paymentsRepository.Get(id);
        return payment;
    }
}
