using System.ComponentModel.DataAnnotations;

using PaymentGateway.Api.Helpers;

namespace PaymentGateway.Api.Models.Attributes;

public class CurrencyCodeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext _)
    {
        if (value is string currencyCode)
        {
            if (currencyCode.Length != 3)
            {
                return new ValidationResult("Currency code must be of length 3");
            }

            return CurrencyCodes.CurrencyCodesSet.Contains(currencyCode)
                ? ValidationResult.Success
                : new ValidationResult($"Currency code '{currencyCode}' is not a valid currency");
        }
        else
        {
            return new ValidationResult("Currency code must be string");
        }
    }
}
