using System.Text.Json.Serialization;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Models.Responses;

public class PostPaymentResponse
{
    [JsonConstructor]
    public PostPaymentResponse(Guid id, PaymentStatus status, string cardNumberLastFour, int expiryMonth, int expiryYear, string currency, int amount)
    {
        Id = id;
        Status = status;
        CardNumberLastFour = cardNumberLastFour;
        ExpiryMonth = expiryMonth;
        ExpiryYear = expiryYear;
        Currency = currency;
        Amount = amount;
    }

    public PostPaymentResponse(Guid id, PostPaymentRequest request, PaymentStatus status)
    {
        Id = id;
        Status = status;
        CardNumberLastFour = request.CardNumber![^4..];
        ExpiryMonth = request.ExpiryMonth;
        ExpiryYear = request.ExpiryYear;
        Currency = request.Currency!;
        Amount = request.Amount;
    }

    public Guid Id { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentStatus Status { get; set; }

    public string CardNumberLastFour { get; set; }

    public int ExpiryMonth { get; set; }

    public int ExpiryYear { get; set; }

    public string Currency { get; set; }

    public int Amount { get; set; }
}
