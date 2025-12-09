using System.Collections.Immutable;
using System.Text.Json.Serialization;
using BytChineseSteam.Models.Enums;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public class Promotion
{
    public static readonly Extent<Promotion> Extent = new();
    
    public string Name { get; set; }
    public double DiscountPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PromotionStatus Status { get; set; }
    
    private readonly HashSet<Key> _keys = new();
    
    public ImmutableHashSet<Key> Keys => _keys.ToImmutableHashSet();
    
    [JsonIgnore]
    public Manager Manager { get; private set; }
    
    public Promotion(string name, double discountPercent, DateTime startDate, 
        DateTime endDate, PromotionStatus status, Key initialKey, Manager manager)
    {
        Name = name;
        DiscountPercent = discountPercent;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        
        if (manager == null)
            throw new ArgumentNullException(nameof(manager), "A promotion must have a Manager.");
        Manager = manager;
        Manager.AddPromotion(this);
        
        if (initialKey == null)
            throw new ArgumentNullException(
                nameof(initialKey), "A promotion must be associated with at least one Key.");
        AddKey(initialKey);
        
        Extent.Add(this);
    }
    
    public void ChangeManager(Manager newManager)
    {
        if (newManager == null) throw new ArgumentNullException(nameof(newManager));
        
        if (Manager == newManager) return;
        Manager.RemovePromotion(this);
        Manager = newManager;
        Manager.AddPromotion(this);
    }
    
    // key association
    public void AddKey(Key key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        
        if (_keys.Contains(key)) return;

        _keys.Add(key);
        
        key.AddPromotion(this);
    }

    public void RemoveKey(Key key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (!_keys.Contains(key)) return;
        
        if (_keys.Count <= 1)
        {
            throw new InvalidOperationException(
                $"Cannot remove the last Key from Promotion '{Name}'. A Promotion must have at least one Key.");
        }

        _keys.Remove(key);
        
        key.RemovePromotion(this);
    }
    
    public void DeletePromotion()
    {
        if (Status == PromotionStatus.ToBeDeleted){
            Manager.RemovePromotion(this);
            foreach (var key in _keys.ToList())
            {
                RemoveKey(key);
            }

            Extent.Remove(this);
        }
        else
        {
            throw new InvalidOperationException("Cannot remove the promotion. Incorrect status");
        }
    }
}