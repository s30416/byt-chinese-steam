using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using BytChineseSteam.Models.DataAnnotations;
using BytChineseSteam.Models.Enums;
using BytChineseSteam.Models.Exceptions.OrderKey;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;



public class Order
{
    public static readonly Extent<Order> Extent = new();
    
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public double TotalSum { get; set; }

    public Order(DateTime createdAt, OrderStatus status, DateTime completedAt, double totalSum, ICollection<Key> keys)
    {
        CreatedAt = createdAt;
        Status = status;
        CompletedAt = completedAt;
        TotalSum = totalSum;

        foreach (var key in keys)
        {
            AddKey(key);
        }
        
        if (_keys.Count == 0)
        {
            throw new OrderCannotBeEmpty();
        }
    }
    
    // associations

    private readonly HashSet<OrderKey> _keys = [];
    
    [MinItemsCount(1)]
    public ImmutableHashSet<OrderKey> Keys => _keys.ToImmutableHashSet();

    public void AddKey(Key key)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        
        var orderKey = new OrderKey(this, key);
        
        if (_keys.Contains(orderKey))
        {
            throw new KeyExistsInOrderException();
        }

        _keys.Add(orderKey);

        try
        {
            if (!key.Orders.Contains(orderKey))
            {
                key.AddToOrder(this);
            }
        }
        catch (Exception e)
        {
            _keys.Remove(orderKey);
            throw;
        }
    }

    public void RemoveKey(Key key)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        
        var orderKey = _keys.FirstOrDefault(k => k.Key == key);
        
        if (orderKey == null)
        {
            throw new KeyDoesNotExistInOrderException();
        }
        
        // order multiplicity logic
        if (_keys.Count == 1)
        {
            throw new OrderCannotBeEmpty();
        }
        
        // removing
        _keys.Remove(orderKey);

        try
        {
            if (key.Orders.Contains(orderKey))
            {
                key.RemoveFromOrder(this);
            }
        }
        catch (Exception e)
        {
            _keys.Add(orderKey);
            throw;
        }
    }
}