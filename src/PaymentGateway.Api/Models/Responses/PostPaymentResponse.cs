using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Models.Responses;

public class PostPaymentResponse
{
    public PostPaymentResponse() { }

    public PostPaymentResponse(Guid id, PostPaymentRequest request, PaymentStatus status)
    {
        Id = id;
        Status = status;
        CardNumberLastFour = request.CardNumber.Substring(request.CardNumber.Length - 4);
        ExpiryMonth = request.ExpiryMonth;
        ExpiryYear = request.ExpiryYear;
        Currency = request.Currency;
        Amount = request.Amount;
    }

    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public string CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
}
