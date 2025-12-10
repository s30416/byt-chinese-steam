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
    
    // for association to Promotion.cs
    [JsonIgnore]
    private readonly HashSet<Promotion> _promotions = new();

    [JsonIgnore]
    public ImmutableHashSet<Promotion> Promotions => _promotions.ToImmutableHashSet();

    [MinLength(1)] [Required] public string AccessKey { get; set; }

    [NonNegative] [Required] public decimal OriginalPrice { get; set; }

    [DateNotInFuture] [Required] public DateTime CreatedAt { get; set; }

    [NonNegative] [Required] public decimal PriceIncrease { get; set; }

    [NonEmpty(isEnumerable: true)] [Required] public List<string> Benefits { get; set; }
    
    [Required]
    public Game Game { get; private set; }
    
    [Required]
    [JsonInclude]
    public Admin Creator { get; private set; }

    public Key(Game game, Admin creator, string accessKey, decimal originalPrice, DateTime createdAt, decimal priceIncrease,
        List<string> benefits)
    {
        if (game == null)
        {
            throw new ArgumentNullException(nameof(game), "Key cannot exist without a Game.");
        }
        
        if (creator == null)
        {
            throw new ArgumentNullException(nameof(creator), "Key must be created by an Admin.");
        }

        Game = game;
        Creator = creator;
        
        AccessKey = accessKey;
        OriginalPrice = originalPrice;
        CreatedAt = createdAt;
        PriceIncrease = priceIncrease;
        Benefits = benefits;

        Game.AddKey(this);
        Creator.AddCreatedKey(this);

        Extent.Add(this);
    }

    // class methods from diagram
    // ...

    public void AddGame(Game game)
    {
        if (game == null) throw new ArgumentNullException(nameof(game), "Key cannot exist without a Game.");

        if (game != this.Game)
        {
          this.Game = game;  
        }
        
        game.AddKey(this);
        
    }

    public void DeleteKey()
    {
        Extent.Remove(this);
        
        Game.RemoveKey(this);
        
        Creator.RemoveCreatedKey(this);

        foreach (var ok in _orders)
        {
            RemoveFromOrder(ok.Order);
        }
        
        foreach (var promo in _promotions.ToList())
        {
            promo.RemoveKey(this);
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
    
    // promotion association
    public void AddPromotion(Promotion promotion)
    {
        if (promotion == null) throw new ArgumentNullException(nameof(promotion));

        // prevent duplicate addition and infinite recursion
        if (_promotions.Contains(promotion)) return;

        _promotions.Add(promotion);

        try
        {
            promotion.AddKey(this);
        }
        catch (Exception)
        {
            _promotions.Remove(promotion);
            throw;
        }
    }

    public void RemovePromotion(Promotion promotion)
    {
        if (promotion == null) throw new ArgumentNullException(nameof(promotion));

        if (!_promotions.Contains(promotion)) return;

        // local remove
        _promotions.Remove(promotion);

        try
        {
            // will throw InvalidOperationException if it violates Promotion's 1..* constraint
            promotion.RemoveKey(this);
        }
        catch (Exception)
        {
            _promotions.Add(promotion);
            throw;
        }
    }
}