using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models.DataAnnotations;
using BytChineseSteam.Models.Exceptions.OrderKey;
using BytChineseSteam.Repository.Extent;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models;

public class Key : Limited
{
    public static readonly Extent<Key> Extent = new ();

    [MinLength(1)] [Required] public string AccessKey { get; set; }

    [NonNegative] [Required] public decimal OriginalPrice { get; set; }

    [DateNotInFuture] [Required] public DateTime CreatedAt { get; set; }

    [NonNegative] [Required] public decimal PriceIncrease { get; set; }

    [NonEmpty(isEnumerable: true)] [Required] public List<string> Benefits { get; set; }
    
    [Required]
    public Game Game { get; private set; }

    public Key(Game game, string accessKey, decimal originalPrice, DateTime createdAt, decimal priceIncrease,
        List<string> benefits)
    {
        if (game == null)
        {
            throw new ArgumentNullException(nameof(game), "Key cannot exist without a Game.");
        }

        Game = game;
        
        AccessKey = accessKey;
        OriginalPrice = originalPrice;
        CreatedAt = createdAt;
        PriceIncrease = priceIncrease;
        Benefits = benefits;

        Game.AddKey(this);

        Extent.Add(this);
    }

    // class methods from diagram
    // ...

    public void DeleteKey()
    {
        Extent.Remove(this);
        
        Game.RemoveKey(this);

        foreach (var ok in _orders)
        {
            RemoveFromOrder(ok.Order);
        }
    }


    public decimal GetCurrentPrice()
    {
        return OriginalPrice + PriceIncrease;
    }
    
    // associations
    [JsonIgnore]
    private readonly HashSet<OrderKey> _orders = [];

    [JsonIgnore]
    public ImmutableHashSet<OrderKey> Orders => _orders.ToImmutableHashSet();
    
    public void AddToOrder(Order order)
    {
        ArgumentNullException.ThrowIfNull(order, nameof(order));
        
        var orderKey = new OrderKey(order, this);
        
        if (_orders.Contains(orderKey))
        {
            throw new KeyExistsInOrderException();
        }

        // adding
        _orders.Add(orderKey);

        try
        {
            if (!order.Keys.Contains(orderKey))
            {
                order.AddKey(this);
            }
        }
        catch (Exception e)
        {
            _orders.Remove(orderKey);
            throw;
        }
    }

    public void RemoveFromOrder(Order order)
    {
        ArgumentNullException.ThrowIfNull(order, nameof(order));

        var orderKey = _orders.FirstOrDefault(o => o.Order == order);
        
        if (orderKey == null)
        {
            throw new KeyDoesNotExistInOrderException();
        }

        // removing
        _orders.Remove(orderKey);

        try
        {
            if (order.Keys.Contains(orderKey))
            {
                order.RemoveKey(this);
            }
        }
        catch (Exception e)
        {
            _orders.Add(orderKey);
            throw;
        }
    }
}