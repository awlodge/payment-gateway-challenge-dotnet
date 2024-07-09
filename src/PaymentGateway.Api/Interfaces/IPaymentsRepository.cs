using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Interfaces
{
    public interface IPaymentsRepository
    {
        Task Add(PostPaymentResponse payment);
        Task<PostPaymentResponse?> Get(Guid id);
    }
}