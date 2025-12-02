using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models.DataAnnotations;
using BytChineseSteam.Repository.Extent;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models;

public class Key : Limited
{
    public static readonly Extent<Key> Extent = new ();
    
    [Required] 
    [JsonInclude]
    public Game Game { get; private set; }
    private Key() { }

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

    public void DeleteKey()
    {
        Extent.Remove(this);
        
        Game.RemoveKey(this);
    }

    [MinLength(1)] [Required] public string AccessKey { get; set; }

    [NonNegative] [Required] public decimal OriginalPrice { get; set; }

    [DateNotInFuture] [Required] public DateTime CreatedAt { get; set; }

    [NonNegative] [Required] public decimal PriceIncrease { get; set; }

    [NonEmpty(isEnumerable: true)] [Required] public List<string> Benefits { get; set; }


    public decimal GetCurrentPrice()
    {
        return OriginalPrice + PriceIncrease;
    }
}