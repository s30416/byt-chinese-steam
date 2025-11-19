using System.ComponentModel.DataAnnotations;

namespace BytChineseSteam.Models.DataAnnotations;

/// <summary>
/// Checks if date in the field is not bigger than DateTime.Now
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NonEmpty(bool isArray = false) : ValidationAttribute("{0} is not valid")
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        if (isArray && !value.GetType().IsArray)
        {
            return new ValidationResult("{0} is not an array");
        }

        if (!isArray && value.GetType().IsArray)
        {
            return new ValidationResult("{0} is array");
        }

        if (isArray)
        {
            var elementType = value.GetType().GetElementType();

            if (elementType != typeof(string))
            {
                return new ValidationResult("{0} is not an array of strings");
            }

            foreach (var item in (value as IEnumerable<string>)!)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    return new ValidationResult("{0} contains empty strings");
                }
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(value as string))
            {
                return new ValidationResult("{0} contains empty strings");
            }
        }

        return ValidationResult.Success;
    }
}