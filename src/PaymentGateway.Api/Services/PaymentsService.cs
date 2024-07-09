using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsService
{
    private readonly IPaymentsRepository _paymentsRepository;

    public PaymentsService(IPaymentsRepository paymentsRepository)
    {
        _paymentsRepository = paymentsRepository;
    }

    public async Task<PostPaymentResponse> ProcessPaymentAsync(PostPaymentRequest request)
    {
        // TODO: Authorize payment via bank.

        // Store payment.
        var payment = new PostPaymentResponse(Guid.NewGuid(), request, PaymentStatus.Authorized);
        await _paymentsRepository.Add(payment);
        return payment;
    }

    public async Task<PostPaymentResponse?> GetPaymentAsync(Guid id)
    {
        var payment = await _paymentsRepository.Get(id);
        return payment;
    }
}
