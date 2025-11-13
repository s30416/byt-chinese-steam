using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace BytChineseSteam.Models.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public class ValidEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var email = value as string;

        if (string.IsNullOrWhiteSpace(email))
        {
            return new ValidationResult("Email address cannot be empty.");
        }

        try
        {
            var mailAddress = new MailAddress(email);
            
            return ValidationResult.Success;
        }
        catch (FormatException)
        {
            return new ValidationResult($"'{email}' is not a valid email address.", 
                new[] { validationContext.MemberName }!);
        }
    }
}