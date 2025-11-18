using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models;

public class Admin : Employee
{
    public static readonly decimal GameBonus = 500;
    
    private static List<Admin> _admins = new();

    protected internal Admin(Name name) : base(name)
    {
        // extent
        AddAdmin(this);
    }

    protected internal Admin(Name name, decimal salary) : base(name, salary)
    {
        // extent
        AddAdmin(this);
    }

    [JsonConstructor]
    private Admin()
    {
        // extent
        AddAdmin(this);
    }

    // extent methods
    public static ReadOnlyCollection<Admin> GetAdmins()
    {
        return _admins.AsReadOnly();
    }
    
    private static void AddAdmin(Admin admin)
    {
        if (admin == null)
            throw new ArgumentException($"The given employee cannot be null");
        
        _admins.Add(admin);
    }
    
    // class methods
}