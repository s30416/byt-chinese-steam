using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models;

public class Manager : Employee
{
    public static readonly decimal PromotionBonus = 100;
    
    private static List<Manager> _managers = new();
    
    private readonly HashSet<Promotion> _promotions = new();
    
    [JsonIgnore]
    public IReadOnlyCollection<Promotion> Promotions => _promotions.ToList().AsReadOnly();

    // extent methods
    
    [JsonConstructor]
    public Manager(Name name, string email, string phoneNumber, string hashedPassword, decimal? salary, SuperAdmin? creator = null)
        : base(name, email, phoneNumber, hashedPassword, salary, creator)
    {
        AddManager(this);
    }

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