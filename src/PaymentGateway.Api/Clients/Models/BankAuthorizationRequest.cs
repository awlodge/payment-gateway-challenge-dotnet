using System.Text.Json.Serialization;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Clients.Models;

internal class BankAuthorizationRequest
{
    [JsonConstructor]
    public BankAuthorizationRequest(string cardNumber, string expiryDate, string currency, int amount, string cvv)
    {
        CardNumber = cardNumber;
        ExpiryDate = expiryDate;
        Currency = currency;
        Amount = amount;
        Cvv = cvv;
    }

    public BankAuthorizationRequest(PostPaymentRequest postPaymentRequest)
    {
        CardNumber = postPaymentRequest.CardNumber!;
        ExpiryDate = $"{postPaymentRequest.ExpiryMonth}/{postPaymentRequest.ExpiryYear}";
        Currency = postPaymentRequest.Currency!;
        Amount = postPaymentRequest.Amount!;
        Cvv = postPaymentRequest.Cvv!;
    }

    [JsonPropertyName("card_number")]
    public string CardNumber { get; set; }

    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("cvv")]
    public string Cvv { get; set; }
}
