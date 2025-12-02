using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models.DataAnnotations;
using BytChineseSteam.Models.Exceptions.OrderKey;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public class Key : Limited
{
    public static readonly Extent<Key> Extent = new ();

    [MinLength(1)] [Required] public string AccessKey { get; set; }

    [NonNegative] [Required] public decimal OriginalPrice { get; set; }

    [DateNotInFuture] [Required] public DateTime CreatedAt { get; set; }

    [NonNegative] [Required] public decimal PriceIncrease { get; set; }

    [NonEmpty(isEnumerable: true)] [Required] public List<string> Benefits { get; set; }
    
    public Key(string accessKey, decimal originalPrice, DateTime createdAt, decimal priceIncrease,
        List<string> benefits)
    {
        AccessKey = accessKey;
        OriginalPrice = originalPrice;
        CreatedAt = createdAt;
        PriceIncrease = priceIncrease;
        Benefits = benefits;

        Extent.Add(this);
    }
    
    // class methods from diagram
    // ...

    public decimal GetCurrentPrice()
    {
        return OriginalPrice + PriceIncrease;
    }
    
    // associations
    private readonly HashSet<OrderKey> _orders = [];

    public ImmutableHashSet<OrderKey> Orders => _orders.ToImmutableHashSet();
    public void AddToOrder(Order order)
    {
        var orderKey = new OrderKey(order, this);
        
        if (!_orders.Add(orderKey))
        {
            throw new KeyExistsInOrderException();
        }

        if (!order.Keys.Contains(orderKey))
        {
            order.AddKey(this);
        }
    }

    public void RemoveFromOrder(Order order)
    {
        var orderKey = new OrderKey(order, this);
        
        if (!_orders.Remove(orderKey))
        {
            throw new KeyDoesNotExistInOrderException();
        }

        if (order.Keys.Contains(orderKey))
        {
            order.RemoveKey(this);
        }
    }
}