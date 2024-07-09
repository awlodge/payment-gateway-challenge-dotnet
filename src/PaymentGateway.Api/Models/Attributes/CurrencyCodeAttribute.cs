﻿using System.ComponentModel.DataAnnotations;

using PaymentGateway.Api.Helpers;

namespace PaymentGateway.Api.Models;

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

            if (!CurrencyCodes.CurrencyCodesSet.Contains(currencyCode))
            {
                return new ValidationResult($"Currency code {currencyCode} is not a valid currency");
            }

            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult("Currency code must be string");
        }
    }
}