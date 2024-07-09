using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Attributes;

public class NumericStringAttribute : ValidationAttribute
{
    public int MinLength { get; }
    public int MaxLength { get; }

    public NumericStringAttribute(int minLength, int maxLength)
    {
        MinLength = minLength;
        MaxLength = maxLength;
    }
    protected override ValidationResult? IsValid(object? value, ValidationContext _)
    {
        if (value is string numericString)
        {
            if (numericString.Length < MinLength || numericString.Length > MaxLength)
            {
                return new ValidationResult($"Numeric string is {numericString.Length} characters long, expected between {MinLength} and {MaxLength} characters");
            }

            if (numericString.Any(c => !char.IsDigit(c)))
            {
                return new ValidationResult("Numeric string contains non-digits");
            }

            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult("Numeric string must be string");
        }
    }
}
