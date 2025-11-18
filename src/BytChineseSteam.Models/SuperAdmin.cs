using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models;

public class SuperAdmin : Employee
{
    private static List<SuperAdmin> _superAdmins = new();

    protected internal SuperAdmin(Name name) : base(name)
    {
        // extent
        AddSuperAdmin(this);
    }

    protected internal SuperAdmin(Name name, decimal salary) : base(name, salary)
    {
        // extent
        AddSuperAdmin(this);
    }

    [JsonConstructor]
    private SuperAdmin()
    {
        // extent
        AddSuperAdmin(this);
    }

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