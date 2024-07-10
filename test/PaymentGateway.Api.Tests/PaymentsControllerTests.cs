﻿using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();
    private readonly HttpClient _client;
    private readonly Mock<IPaymentsRepository> _paymentsRepository = new();
    private readonly Mock<IBankAuthorizationClient> _bankAuthorizationClient = new();

    public PaymentsControllerTests()
    {
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        _client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddMetrics()
                .AddSingleton(_paymentsRepository.Object)
                .AddSingleton(_bankAuthorizationClient.Object)
                .AddSingleton<PaymentsService>()))
            .CreateClient();
    }

    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PostPaymentResponse(
            id: Guid.NewGuid(),
            status: PaymentStatus.Authorized,
            expiryYear: _random.Next(2023, 2030),
            expiryMonth: _random.Next(1, 12),
            amount: _random.Next(1, 10000),
            cardNumberLastFour: _random.Next(1111, 9999).ToString(),
            currency: "GBP");

        _paymentsRepository.Setup(x => x.Get(payment.Id)).ReturnsAsync(payment).Verifiable();

        // Act
        var response = await _client.GetAsync($"/api/Payments/{payment.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        Assert.NotNull(paymentResponse);
        Mock.Verify();
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ProcessesAPaymentSuccessfully()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = 2100,
            Currency = "GBP",
            Amount = 100,
            Cvv = "456",
        };
        _bankAuthorizationClient.Setup(x => x.AuthorizationRequest(It.IsAny<PostPaymentRequest>())).ReturnsAsync(PaymentStatus.Authorized).Verifiable();

        var response = await _client.PostAsync("/api/Payments", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        Assert.NotNull(paymentResponse);
        Assert.Equal(PaymentStatus.Authorized, paymentResponse!.Status);
        Mock.Verify();
    }

    [Fact]
    public async Task ProcessesAPaymentDeclined()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = 2100,
            Currency = "GBP",
            Amount = 100,
            Cvv = "456",
        };
        _bankAuthorizationClient.Setup(x => x.AuthorizationRequest(It.IsAny<PostPaymentRequest>())).ReturnsAsync(PaymentStatus.Declined).Verifiable();

        var response = await _client.PostAsync("/api/Payments", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        Assert.NotNull(paymentResponse);
        Assert.Equal(PaymentStatus.Declined, paymentResponse!.Status);
        Mock.Verify();
    }

    [Theory]
    [InlineData(null, 12, 2100, "GBP", 100)]
    [InlineData("1", 12, 2100, "GBP", 100)]
    [InlineData("1234567890123", 12, 2100, "GBP", 100)]
    [InlineData("12345678901234567890", 12, 2100, "GBP", 100)]
    [InlineData("1234567890123abc", 12, 2100, "GBP", 100)]
    [InlineData("123456789012345", 0, 2100, "GBP", 100)]
    [InlineData("123456789012345", 13, 2100, "GBP", 100)]
    [InlineData("123456789012345", 12, -1, "GBP", 100)]
    [InlineData("123456789012345", 12, 1999, "GBP", 100)]
    [InlineData("123456789012345", 12, 2100, "bad currency", 100)]
    [InlineData("123456789012345", 12, 2100, "ABC", 100)]
    [InlineData("123456789012345", 12, 2100, "GBP", -1)]
    public async Task RejectsInvalidPaymentRequest(string? cardNumber, int expiryMonth, int expiryYear, string? currency, int amount)
    {
        var request = new PostPaymentRequest
        {
            CardNumber = cardNumber,
            ExpiryMonth = expiryMonth,
            ExpiryYear = expiryYear,
            Currency = currency,
            Amount = amount,
            Cvv = "456",
        };

        var response = await _client.PostAsync("/api/Payments", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Returns500OnBankAuthorizationError()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = 2100,
            Currency = "GBP",
            Amount = 100,
            Cvv = "456",
        };
        _bankAuthorizationClient.Setup(x => x.AuthorizationRequest(It.IsAny<PostPaymentRequest>())).ThrowsAsync(new BankAuthorizationException("Dummy message"));

        var response = await _client.PostAsync("/api/Payments", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}