using System.ComponentModel.DataAnnotations;

namespace BytChineseSteam.Models.DataAnnotations;

/// <summary>
/// Checks if the value of numeric type is larger than zero
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NonNegative() : ValidationAttribute("Field {0} must be a positive number")
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        switch (value)
        {
            case null:
                return ValidationResult.Success;
            case IConvertible convertible:
                try
                {
                    var number = convertible.ToDecimal(null);

                    return number >= 0 ? ValidationResult.Success : new ValidationResult(FormatErrorMessage(validationContext.DisplayName),
                        [validationContext.MemberName!]);
                }
                catch (InvalidCastException)
                {
                    return new ValidationResult($"The field {validationContext.DisplayName} must be a valid numeric type", [validationContext.MemberName!]);
                }
            default:
                return new ValidationResult($"The field {validationContext.DisplayName} must be numeric type",
                    [validationContext.MemberName!]);
        }
    }
}