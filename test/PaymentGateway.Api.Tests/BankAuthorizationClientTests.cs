using System.Net;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;
using Moq.Protected;

using PaymentGateway.Api.Clients;
using PaymentGateway.Api.Configuration;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests;

public class BankAuthorizationClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler = new(MockBehavior.Strict);
    private readonly BankAuthorizationClient _bankAuthorizationClient;

    public BankAuthorizationClientTests()
    {
        var mockLogger = new Mock<ILogger<BankAuthorizationClient>>();
        var mockOptions = new Mock<IOptions<BankAuthorizationClientOptions>>();
        mockOptions.SetupGet(x => x.Value).Returns(new BankAuthorizationClientOptions()
        {
            BaseUrl = "https://bank.example.com"
        });
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _bankAuthorizationClient = new(mockLogger.Object, httpClient, mockOptions.Object);
    }

    [Fact]
    public async Task AuthorizesAPayment()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = 2023,
            Currency = "GBP",
            Amount = 100,
            Cvv = "456",
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"authorized\": true,\"authorization_code\": \"0bb07405-6d44-4b50-a14f-7ae0beff13ad\"}"),
            })
            .Verifiable();

        var result = await _bankAuthorizationClient.AuthorizationRequest(request);
        Assert.Equal(PaymentStatus.Authorized, result);
    }

    [Fact]
    public async Task DeclinesAPayment()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = 2023,
            Currency = "GBP",
            Amount = 100,
            Cvv = "456",
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"authorized\": false}"),
            })
            .Verifiable();

        var result = await _bankAuthorizationClient.AuthorizationRequest(request);
        Assert.Equal(PaymentStatus.Declined, result);
    }

    [Fact]
    public async Task ThrowsOnHttpClientError()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = 2023,
            Currency = "GBP",
            Amount = 100,
            Cvv = "456",
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Simulated error"))
            .Verifiable();

        await Assert.ThrowsAsync<BankAuthorizationException>(() => _bankAuthorizationClient.AuthorizationRequest(request));
    }

    [Fact]
    public async Task ThrowsOnErrorResponse()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = 2023,
            Currency = "GBP",
            Amount = 100,
            Cvv = "456",
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
            })
            .Verifiable();

        await Assert.ThrowsAsync<BankAuthorizationException>(() => _bankAuthorizationClient.AuthorizationRequest(request));
    }

    [Fact]
    public async Task ThrowsOnEmptyResponse()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = 2023,
            Currency = "GBP",
            Amount = 100,
            Cvv = "456",
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
            })
            .Verifiable();

        await Assert.ThrowsAsync<BankAuthorizationException>(() => _bankAuthorizationClient.AuthorizationRequest(request));
    }
}
