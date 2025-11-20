using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models;

public class Manager(Name name, string email, string phoneNumber, string hashedPassword, decimal? salary)
    : Employee(name, email, phoneNumber, hashedPassword, salary)
{
    public static readonly decimal PromotionBonus = 100;
    
    private static List<Manager> _managers = new();

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