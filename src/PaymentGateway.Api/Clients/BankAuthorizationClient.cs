using PaymentGateway.Api.Clients.Models;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Clients;

public class BankAuthorizationClient : IBankAuthorizationClient
{
    private readonly HttpClient _httpClient;

    // TODO: Put this in config.
    private const string _baseUrl = "http://localhost:8080/payments";

    public BankAuthorizationClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_baseUrl);
    }

    public async Task<PaymentStatus> AuthorizationRequest(PostPaymentRequest request)
    {
        var bankAuthorizationRequest = new BankAuthorizationRequest(request);
        var response = await _httpClient.PostAsJsonAsync(_baseUrl, bankAuthorizationRequest);

        if (response == null)
        {
            throw new Exception("Bank authorization request failed: no response");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Bank authorization request failed: " + response.ReasonPhrase);
        }

        if (response.Content == null)
        {
            throw new Exception("Bank authorization request failed: response body is empty");
        }

        var responseContent = await response.Content.ReadFromJsonAsync<BankAuthorizationResponse>();
        if (responseContent == null)
        {
            throw new Exception("Bank authorization request failed: failed to parse response body");
        }

        return responseContent.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
    }
}
