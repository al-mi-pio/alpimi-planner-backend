using System.ComponentModel.DataAnnotations;
using AlpimiAPI.Locales;
using AlpimiAPI.Utilities;
using Microsoft.Extensions.Localization;

public class LocalizedRequiredAttribute : RequiredAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var _str = (IStringLocalizer)
            validationContext.GetService(typeof(IStringLocalizer<Errors>))!;
        var _strFields = (IStringLocalizer)
            validationContext.GetService(typeof(IStringLocalizer<Fields>))!;

        string errorMessage = _str["requiredField", _strFields[validationContext.DisplayName]];

        if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
        {
            return new ValidationResult(
                $"[FieldErrorObject]<{TextUtils.FirstLetterToLower(validationContext.DisplayName)}>{errorMessage}"
            );
        }

        return ValidationResult.Success!;
    }
}
