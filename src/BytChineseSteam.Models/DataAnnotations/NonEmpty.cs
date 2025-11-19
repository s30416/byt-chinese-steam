using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace BytChineseSteam.Models.DataAnnotations;

/// <summary>
/// Checks if date in the field is not bigger than DateTime.Now
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NonEmpty(bool isEnumerable = false) : ValidationAttribute("{0} is not valid")
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        if (isEnumerable && value is not IEnumerable)
        {
            Console.WriteLine($"{isEnumerable} {value.GetType().Name}");
            return new ValidationResult($"{validationContext.DisplayName} is not enumerable",
                [validationContext.DisplayName]);
        }

        if (!isEnumerable && value is IEnumerable)
        {
            return new ValidationResult($"{validationContext.DisplayName} is enumerable",
                [validationContext.DisplayName]);
        }

        if (isEnumerable)
        {
            foreach (var item in (value as IEnumerable)!)
            {
                if (string.IsNullOrWhiteSpace(item as string))
                {
                    return new ValidationResult($"{validationContext.DisplayName} contains empty strings",
                        [validationContext.DisplayName]);
                }
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(value as string))
            {
                return new ValidationResult($"{validationContext.DisplayName} contains empty strings");
            }
        }

        return ValidationResult.Success;
    }
}