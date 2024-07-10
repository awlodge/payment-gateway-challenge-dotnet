using Microsoft.Extensions.Options;

using PaymentGateway.Api.Clients.Models;
using PaymentGateway.Api.Configuration;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Clients;

public class BankAuthorizationClient : IBankAuthorizationClient
{
    private readonly ILogger<BankAuthorizationClient> _logger;
    private readonly HttpClient _httpClient;

    public BankAuthorizationClient(ILogger<BankAuthorizationClient> logger, HttpClient httpClient, IOptions<BankAuthorizationClientOptions> options)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
    }

    public async Task<PaymentStatus> AuthorizationRequest(PostPaymentRequest request)
    {
        _logger.LogDebug("Sending new bank authorization request");
        var bankAuthorizationRequest = new BankAuthorizationRequest(request);
        var response = await _httpClient.PostAsJsonAsync("/payments", bankAuthorizationRequest);

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

        _logger.LogDebug("Bank authorization response: {@response}", responseContent);
        return responseContent.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
    }
}
