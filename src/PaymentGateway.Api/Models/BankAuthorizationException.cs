namespace PaymentGateway.Api.Models;

[Serializable]
public class BankAuthorizationException : Exception
{
    public BankAuthorizationException()
    {
    }

    public BankAuthorizationException(string? message) : base(message)
    {
    }

    public BankAuthorizationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}