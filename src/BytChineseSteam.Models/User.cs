using System.ComponentModel.DataAnnotations;

namespace BytChineseSteam.Models;

public abstract class User
{
 
    [Required]
    public Name Name { get; set; } = null!;
    
    [Required]
    [ValidEmail]
    public string Email { get; set; } = null!;

    [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$")]
    public string PhoneNumber { get; set; } = null!;
    
    
    public string HashedPassword { get; set; } = null!;


    protected User(Name name, string email, string phoneNumber, string hashedPassword)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        HashedPassword = hashedPassword;
    }
    
    public User() {}
    
}


public class Name
{
    [Required]
    public string FirstName { get; set; } = null!;
    
    
    [Required]
    public string LastName { get; set; } = null!;

    public Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
    
    public Name() {}
    
}