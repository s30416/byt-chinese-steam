using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models.DataAnnotations;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public abstract class User
{

    private static readonly Extent<User> Extent = new();
 
    [Required]
    public Name Name { get; set; } = null!;
    
    [Required]
    [ValidEmail]
    public string Email { get; set; } = null!;

    [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$")]
    public string PhoneNumber { get; set; } = null!;
    
    [Required]
    [MinLength(8)]
    public string HashedPassword { get; set; } = null!;
    


    protected User(Name name, string email, string phoneNumber, string hashedPassword)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        HashedPassword = hashedPassword;
    }
    

    
    public static User CreateUser(User newUser)
    {
        if (Extent.All().Any(u => u.Email.Equals(newUser.Email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("User already exists");
        }
        
        Extent.Add(newUser);
        return newUser;
    }

    
    public static IReadOnlyList<User> ViewAllUsers()
    {
        return Extent.All();
    }
    
    
    public static User? GetUserByEmail(string email)
    {
        return Extent.All().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
    
    public static User? UpdateUser(string email, Name newName, string newPhoneNumber)
    {
        
        var userToUpdate = GetUserByEmail(email);
        if (userToUpdate == null)
        {
            return null;
        }
        
        userToUpdate.Name = newName;
        userToUpdate.PhoneNumber = newPhoneNumber;
        
        return userToUpdate;
    }


    public static bool DeleteUser(string email)
    {
        var userToDelete = GetUserByEmail(email);
        if (userToDelete != null)
        {
            Extent.Remove(userToDelete);
            return true;
        }
        return false;
    }
    
}


public class Name
{
    [Required]
    [MinLength(1)]
    public string FirstName { get; set; } = null!;
    
    [Required]
    [MinLength(1)]
    public string LastName { get; set; } = null!;

    public Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
    
    public Name() {}
    
}