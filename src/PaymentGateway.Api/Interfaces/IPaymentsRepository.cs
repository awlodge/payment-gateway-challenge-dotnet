using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Interfaces
{
    public interface IPaymentsRepository
    {
        /// <summary>
        /// Store a payment in the repository.
        /// </summary>
        /// <param name="payment">The payment to store</param>
        Task Add(PostPaymentResponse payment);

        /// <summary>
        /// Get a payment from the repository.
        /// </summary>
        /// <param name="id">GUID identifying the payment.</param>
        /// <returns>The payment object.</returns>
        Task<PostPaymentResponse?> Get(Guid id);
    }
}