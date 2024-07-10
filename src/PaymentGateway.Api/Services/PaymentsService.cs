using System.Diagnostics;
using System.Diagnostics.Metrics;

using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsService
{
    private readonly ILogger<PaymentsService> _logger;
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IBankAuthorizationClient _bankAuthorizationClient;
    private readonly Counter<int> _paymentsProcessedCounter;
    private readonly Histogram<double> _paymentProcessDurationHistogram;
    private readonly Histogram<int> _paymentsProcessedAmountHistogram;

    public PaymentsService(ILogger<PaymentsService> logger, IPaymentsRepository paymentsRepository, IBankAuthorizationClient bankAuthorizationClient, IMeterFactory meterFactory)
    {
        _logger = logger;
        _paymentsRepository = paymentsRepository;
        _bankAuthorizationClient = bankAuthorizationClient;

        var meter = meterFactory.Create("Payments");
        _paymentsProcessedCounter = meter.CreateCounter<int>("payment.processed.total", description: "Number of processed payments");
        _paymentProcessDurationHistogram = meter.CreateHistogram<double>("payment.processed.duration", unit: "milliseconds", description: "Duration of payment processing");
        _paymentsProcessedAmountHistogram = meter.CreateHistogram<int>("payment.processed.amount", description: "Amounts of processed payments");
    }

    public async Task<PostPaymentResponse> ProcessPaymentAsync(PostPaymentRequest request)
    {
        var sw = new Stopwatch();
        sw.Start();
        var paymentId = Guid.NewGuid();
        _logger.LogInformation("Processing new payment request: {@id}", paymentId);

        // Authorize payment via bank.
        var paymentStatus = await _bankAuthorizationClient.AuthorizationRequest(request);
        _logger.LogInformation("Payment with id {@id} status: {@authorized}", paymentId, paymentStatus);

        // Store payment.
        var payment = new PostPaymentResponse(Guid.NewGuid(), request, paymentStatus);
        await _paymentsRepository.Add(payment);
        _logger.LogInformation("Payment with id {@id} stored", paymentId);
        sw.Stop();
        RecordMetrics(payment, sw.Elapsed.TotalMilliseconds);
        return payment;
    }

    public async Task<PostPaymentResponse?> GetPaymentAsync(Guid id)
    {
        _logger.LogInformation("Get payment with id {@id}", id);
        var payment = await _paymentsRepository.Get(id);
        return payment;
    }

    private void RecordMetrics(PostPaymentResponse payment, double duration)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("payment.status", payment.Status.ToString()),
            new("payment.currency", payment.Currency)
        };
        _paymentsProcessedCounter.Add(1, tags);
        _paymentProcessDurationHistogram.Record(duration, tags);
        _paymentsProcessedAmountHistogram.Record(payment.Amount, tags);
    }
}
