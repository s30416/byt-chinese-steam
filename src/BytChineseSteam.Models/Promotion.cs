using System.Collections.Immutable;
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
    
    public Promotion(string name, double discountPercent, DateTime startDate, DateTime endDate, PromotionStatus status, Key initialKey)
    {
        Name = name;
        DiscountPercent = discountPercent;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        
        if (initialKey == null)
            throw new ArgumentNullException(
                nameof(initialKey), "A promotion must be associated with at least one Key.");

        AddKey(initialKey);
        Extent.Add(this);
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
}