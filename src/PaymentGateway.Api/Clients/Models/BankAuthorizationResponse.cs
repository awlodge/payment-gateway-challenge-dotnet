using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Clients.Models;

internal class BankAuthorizationResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }

    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; }
}
