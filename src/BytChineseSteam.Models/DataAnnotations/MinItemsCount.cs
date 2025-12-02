using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace BytChineseSteam.Models.DataAnnotations;

/// <summary>
/// Checks if number of elements in ICollection is greater than `value`
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MinItemsCount(int minCount) : ValidationAttribute("{0} is not valid")
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        if (value is not ICollection collection)
        {
            return new ValidationResult($"{validationContext.DisplayName} is not collection",
                [validationContext.DisplayName]);
        }
        
        if (collection.Count < minCount) 
            return new ValidationResult(
                $"{validationContext.DisplayName} has too few items {collection.Count} < {minCount} "
            );

        return ValidationResult.Success;
    }
}