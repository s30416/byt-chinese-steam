using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.InteropServices.Marshalling;
using BytChineseSteam.Models.DataAnnotations;
using BytChineseSteam.Models.Exceptions.OrderKey;
using BytChineseSteam.Repository.Extent;
using System.Text.Json.Serialization;
using BytChineseSteam.Models.Enums;

namespace BytChineseSteam.Models;

public abstract class Key
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
    
    [Required]
    public Game Game { get; private set; }
    
    [Required]
    [JsonInclude]
    public Admin Creator { get; private set; }
    
    /*
     * Regional-Key-UniversalPriceIncrease inheritance flattening
     */
    // Regional
    public KeyLocalization  Localization { get; }

    private string? _country;
    public string? Country { get => _country; set => SetCountry(value); }
    
    // Universal 
    private decimal? _universalPriceIncrease;
    [NonNegative] public decimal? UniversalPriceIncrease { get => _universalPriceIncrease; set => SetUniversalPriceIncrease(value); }

    // Universal Key constructor
    public Key(Game game, Admin creator, string accessKey, decimal originalPrice, DateTime createdAt, decimal universalPriceIncrease) : this(game, creator, accessKey, originalPrice, createdAt)
    {
        Localization = KeyLocalization.Universal;
        SetUniversalPriceIncrease(universalPriceIncrease);
    }
    
    // Regional Key constructor
    public Key(Game game, Admin creator, string accessKey, decimal originalPrice, DateTime createdAt, string country) : this(game, creator, accessKey, originalPrice, createdAt)
    {
        Localization = KeyLocalization.Regional;
        SetCountry(country);
    }

    // base constructor
    private Key(Game game, Admin creator, string accessKey, decimal originalPrice, DateTime createdAt) {
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
    }
    
    private void SetCountry(string country)
    {
        if (Localization != KeyLocalization.Regional)
        {
            throw new ArgumentException("Cannot set country for non Regional key");
        }

        if (country.Length == 0)
        {
            throw new ArgumentException("Cannot set country for empty string");
        }

        if (country == null)
        {
            throw new ArgumentNullException(nameof(country), "Country cannot be null");
        }

        _country = country;
    }

    private void SetUniversalPriceIncrease(decimal? priceIncrease)
    {
        if (Localization != KeyLocalization.Universal)
        {
            throw new ArgumentException("Cannot set price increase for non Universal key");
        }

        if (priceIncrease == null)
        {
            throw new ArgumentNullException(nameof(priceIncrease), "Price increase must not be null.");
        }

        _universalPriceIncrease = priceIncrease;
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

    internal void RemoveGame(Game game)
    {
        if (game == null) throw new ArgumentNullException(nameof(game), "Key cannot exist without a Game.");
        
        if (game != this.Game)
        {
            this.Game = game;
        }
        game.RemoveKey(this);
    }
    
    
    public virtual decimal GetCurrentPrice()
    {
        var p = OriginalPrice;

        if (UniversalPriceIncrease != null)
        {
            p += UniversalPriceIncrease.Value;
        }
        return p;
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