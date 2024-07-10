using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Interfaces;

public interface IBankAuthorizationClient
{
    /// <summary>
    /// Send an authorization request to the bank.
    /// </summary>
    /// <param name="request">Inbound request.</param>
    /// <returns>PaymentStatus.Authorized if the payment was authorized, PaymentStatus.Declined otherwise.</returns>
    Task<PaymentStatus> AuthorizationRequest(PostPaymentRequest request);
}
