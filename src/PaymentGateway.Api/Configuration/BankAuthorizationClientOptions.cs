namespace PaymentGateway.Api.Configuration;

public class BankAuthorizationClientOptions
{
    public const string BankAuthorizationClient = "BankAuthorizationClient";

    /// <summary>
    /// URL to access the bank authorization service.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;
}
