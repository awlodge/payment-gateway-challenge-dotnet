using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository : IPaymentsRepository
{
    public Dictionary<Guid, PostPaymentResponse> Payments = new();

    public async Task Add(PostPaymentResponse payment)
    {
        Payments[payment.Id] = payment;
    }

    public async Task<PostPaymentResponse?> Get(Guid id)
    {
        return Payments.GetValueOrDefault(id);
    }
}