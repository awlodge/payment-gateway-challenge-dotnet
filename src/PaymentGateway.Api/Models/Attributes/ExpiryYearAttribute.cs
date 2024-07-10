using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Attributes;

public class ExpiryYearAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int expiryYear)
        {
            int expiryMonth = (int)validationContext.ObjectType.GetProperty("ExpiryMonth")!.GetValue(validationContext.ObjectInstance)!;

            try
            {
                DateTime expiryDate = new(expiryYear, expiryMonth, 1);
                if (expiryDate < DateTime.Now)
                {
                    return new ValidationResult($"Expiry date must be in the future: {expiryYear}/{expiryMonth:D2}");
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return new ValidationResult($"Invalid expiry date: {expiryYear}/{expiryMonth:D2}");
            }

            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult("Expiry year must be an integer");
        }
    }
}
