using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models.DataAnnotations;

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
    
    private static readonly List<User> _users = new List<User>();


    protected User(Name name, string email, string phoneNumber, string hashedPassword)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        HashedPassword = hashedPassword;
    }
    
    public User() {}


    
    public static User CreateUser(User newUser)
    {
        if (_users.Any(u => u.Email.Equals(newUser.Email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("User already exists");
        }
        
        _users.Add(newUser);
        return newUser;
    }

    
    public static IReadOnlyList<User> ViewAllUsers()
    {
        return _users.AsReadOnly();
    }
    
    
    public static User? GetUserByEmail(string email)
    {
        return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
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
            _users.Remove(userToDelete);
            return true;
        }
        return false;
    }
    
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