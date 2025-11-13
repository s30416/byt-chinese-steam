using System.ComponentModel.DataAnnotations;

namespace BytChineseSteam.Models.DataAnnotations;

/// <summary>
/// Checks if date in the field is not bigger than DateTime.Now
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DateNotInFuture() : ValidationAttribute("Date for {0} cannot be in the future")
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        switch (value)
        {
            case null:
                return ValidationResult.Success;
            case DateTime dateTime:
            {
                if (dateTime <= DateTime.Now) return ValidationResult.Success;
                break;
            }
            default:
                return new ValidationResult($"The field {validationContext.DisplayName} must be a DateTime type");
        }

        return new ValidationResult(
            FormatErrorMessage(validationContext.DisplayName),
            [validationContext.MemberName!]
        );
    }
}