using System.Text.Json;

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

        try
        {
            var response = await _httpClient.PostAsJsonAsync("/payments", bankAuthorizationRequest) ?? throw new BankAuthorizationException("Bank authorization request failed: no response");

            if (!response.IsSuccessStatusCode)
            {
                throw new BankAuthorizationException("Bank authorization request failed: " + response.ReasonPhrase);
            }

            if (response.Content == null)
            {
                throw new BankAuthorizationException("Bank authorization request failed: response body is empty");
            }

            try
            {
                var responseContent = await response.Content.ReadFromJsonAsync<BankAuthorizationResponse?>() ?? throw new BankAuthorizationException("Bank authorization request failed: response body is empty");
                _logger.LogDebug("Bank authorization response: {@response}", responseContent);
                return responseContent.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
            }
            catch (JsonException ex)
            {
                throw new BankAuthorizationException("Failed to parse bank authorization response", ex);
            }
        }
        catch (HttpRequestException ex)
        {
            throw new BankAuthorizationException("Bank authorization request failed", ex);
        }
    }
}
