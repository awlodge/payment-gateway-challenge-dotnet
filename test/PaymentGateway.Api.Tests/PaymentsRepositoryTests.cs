using System.Diagnostics.Metrics;

using Moq;

using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

public class PaymentsRepositoryTests
{
    private readonly Random _random = new();
    private readonly PaymentsRepository _paymentsRepository;

    public PaymentsRepositoryTests()
    {
        var mockMeterFactory = new Mock<IMeterFactory>();
        _paymentsRepository = new PaymentsRepository(mockMeterFactory.Object);
    }

    [Fact]
    public async Task GetOnMissingPaymentReturnsNull()
    {
        var id = new Guid();
        var payment = await _paymentsRepository.Get(id);
        Assert.Null(payment);
    }

    [Fact]
    public async Task CanAddNewPayment()
    {
        var payment = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999).ToString(),
            Currency = "GBP"
        };

        await _paymentsRepository.Add(payment);
        var storedPayment = await _paymentsRepository.Get(payment.Id);
        Assert.NotNull(storedPayment);
        Assert.Equal(payment.Id, storedPayment!.Id);
        Assert.Equal(payment.ExpiryYear, storedPayment.ExpiryYear);
        Assert.Equal(payment.ExpiryMonth, storedPayment.ExpiryMonth);
        Assert.Equal(payment.Amount, storedPayment.Amount);
        Assert.Equal(payment.CardNumberLastFour, storedPayment.CardNumberLastFour);
        Assert.Equal(payment.Currency, storedPayment.Currency);
    }
}
