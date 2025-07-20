using System.ComponentModel.DataAnnotations;
using AlpimiAPI.Locales;
using Microsoft.Extensions.Localization;

public class LocalizedRequiredAttribute : RequiredAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var _str = (IStringLocalizer)
            validationContext.GetService(typeof(IStringLocalizer<Errors>))!;

        string errorMessage = _str["requiredField", validationContext.DisplayName];

        if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
        {
            return new ValidationResult(errorMessage);
        }

        return ValidationResult.Success!;
    }
}
