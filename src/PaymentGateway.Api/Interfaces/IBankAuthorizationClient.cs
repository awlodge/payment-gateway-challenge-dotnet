using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Interfaces;

public interface IBankAuthorizationClient
{
    Task<PaymentStatus> AuthorizationRequest(PostPaymentRequest request);
}
