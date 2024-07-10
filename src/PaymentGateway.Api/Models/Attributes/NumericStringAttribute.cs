using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Attributes;

public class NumericStringAttribute(int minLength, int maxLength) : ValidationAttribute
{
    public int MinLength { get; } = minLength;
    public int MaxLength { get; } = maxLength;

    public override bool IsValid(object? value)
    {
        return value is string numericString
            && numericString.Length >= MinLength
            && numericString.Length <= MaxLength
            && numericString.All(char.IsDigit);
    }
}
