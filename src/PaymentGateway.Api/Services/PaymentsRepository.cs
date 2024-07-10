using System.Diagnostics.Metrics;

using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository : IPaymentsRepository
{
    public Dictionary<Guid, PostPaymentResponse> Payments = [];

    public PaymentsRepository(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("Payments");
        // In unit tests meter is null, but we don't need the metric then.
        ObservableGauge<int>? observableGauge = meter?.CreateObservableGauge("payments.stored.total", () => Payments.Count, description: "Number of stored payments");
    }

    public Task Add(PostPaymentResponse payment)
    {
        Payments[payment.Id] = payment;
        return Task.CompletedTask;
    }

    public Task<PostPaymentResponse?> Get(Guid id)
    {
        return Task.FromResult(Payments.GetValueOrDefault(id));
    }
}