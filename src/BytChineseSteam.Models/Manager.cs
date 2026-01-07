using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public class Manager
{
    
    public static Extent<Manager> Extent = new();
    
    public static readonly decimal PromotionBonus = 100;
    
    private readonly HashSet<Promotion> _promotions = new();
    
    [JsonIgnore]
    public Employee Employee { get; private set; }
    
    [JsonIgnore]
    public IReadOnlyCollection<Promotion> Promotions => _promotions.ToList().AsReadOnly();

    // extent methods
    
    [JsonConstructor]
    public Manager(Employee employee)
    {
        if (employee == null) throw new ArgumentNullException(nameof(employee));
        
        Employee = employee;
        
        Employee.AssignManagerRole(this);

        AddManager(this);
    }

    public static ReadOnlyCollection<Manager> ViewAllManagers()
    {
        return Extent.All();
    }
    
    private static void AddManager(Manager manager)
    {
        if (manager == null)
            throw new ArgumentException($"The given manager cannot be null");
        
        Extent.Add(manager);
    }
    
    // Promotion association
    internal void AddPromotion(Promotion promotion)
    {
        if (promotion == null) throw new ArgumentNullException(nameof(promotion));
        
        if (!_promotions.Contains(promotion))
        {
            _promotions.Add(promotion);
        }
    }

    internal void RemovePromotion(Promotion promotion)
    {
        if (promotion == null) throw new ArgumentNullException(nameof(promotion));
        
        _promotions.Remove(promotion);
    }
    
    // class methods
}