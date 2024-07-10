
using System.ComponentModel.DataAnnotations;

using PaymentGateway.Api.Models.Attributes;

namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest
{
    [Required]
    [NumericString(minLength: 14, maxLength: 19, ErrorMessage = "Card number must be a numeric string between 14 and 19 characters")]
    public string? CardNumber { get; set; }

    [Required]
    [Range(1, 12)]
    public int ExpiryMonth { get; set; }

    [Required]
    [ExpiryYear]
    public int ExpiryYear { get; set; }

    [Required]
    [CurrencyCode]
    public string? Currency { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Amount must be non-negative")]
    public int Amount { get; set; }

    [Required]
    [NumericString(minLength: 3, maxLength: 4, ErrorMessage = "CVV must be a numeric string between 3 and 4 characters")]
    public string? Cvv { get; set; }
}
