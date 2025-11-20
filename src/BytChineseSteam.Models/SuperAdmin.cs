using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models;

public class SuperAdmin(Name name, string email, string phoneNumber, string hashedPassword, decimal? salary)
    : Employee(name, email, phoneNumber, hashedPassword, salary)
{
    private static List<SuperAdmin> _superAdmins = new();

    // extent methods

    public static ReadOnlyCollection<SuperAdmin> ViewAllSuperAdmins()
    {
        return _superAdmins.AsReadOnly();
    }
    
    private static void AddSuperAdmin(SuperAdmin superAdmin)
    {
        if (superAdmin == null)
            throw new ArgumentException($"The given employee cannot be null");
        
        _superAdmins.Add(superAdmin);
    }
    
    // class methods
}