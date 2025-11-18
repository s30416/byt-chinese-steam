using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models;

public class Manager : Employee
{
    public static readonly decimal PromotionBonus = 100;
    
    private static List<Manager> _managers = new();

    protected internal Manager(Name name) : base(name)
    {
        // extent
        AddManager(this);
    }

    protected internal Manager(Name name, decimal salary) : base(name, salary)
    {
        // extent
        AddManager(this);
    }

    [JsonConstructor]
    private Manager()
    {
        // extent
        AddManager(this);
    }
    
    // extent methods
    public static ReadOnlyCollection<Manager> ViewAllManagers()
    {
        return _managers.AsReadOnly();
    }
    
    private static void AddManager(Manager manager)
    {
        if (manager == null)
            throw new ArgumentException($"The given manager cannot be null");
        
        _managers.Add(manager);
    }
    
    // class methods
}