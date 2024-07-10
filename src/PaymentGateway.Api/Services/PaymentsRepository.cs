using System.Diagnostics.Metrics;

using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository : IPaymentsRepository
{
    public Dictionary<Guid, PostPaymentResponse> Payments = new();
    private readonly ObservableGauge<int> _paymentsStoredGauge;

    public PaymentsRepository(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("Payments");
        if (meter != null)
        {
            // In unit tests meter is null, but we don't need the metric then.
            _paymentsStoredGauge = meter.CreateObservableGauge<int>("payments.stored.total", () => Payments.Count, description: "Number of stored payments");
        }
    }

    public async Task Add(PostPaymentResponse payment)
    {
        Payments[payment.Id] = payment;
    }

    public async Task<PostPaymentResponse?> Get(Guid id)
    {
        return Payments.GetValueOrDefault(id);
    }
}