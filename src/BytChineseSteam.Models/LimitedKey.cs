using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;
using BytChineseSteam.Models.DataAnnotations;
using BytChineseSteam.Models.Enums;
using BytChineseSteam.Models.Interfaces;

namespace BytChineseSteam.Models;

public class LimitedKey : Key
{
    public static int ReturnWindow { get; } = 14;
    
    [NotNull]
    [NonNegative]
    public decimal LimitedPriceIncrease { get; set; }
    
    [NotNull]
    [NonEmpty(true)]
    public List<string> Benefits { get; set; }
    
    
    /*
     * Universal Limited Key Key constructor
     */
    public LimitedKey(Game game, IAdmin creator, string accessKey, decimal originalPrice, DateTime createdAt, decimal universalPriceIncrease, List<string> benefits, decimal limitedPriceIncrease)
        : base(game, creator, accessKey, originalPrice, createdAt, universalPriceIncrease)
    {
        LimitedPriceIncrease = limitedPriceIncrease;
        Benefits = benefits;
        
        Game.AddKey(this);
        Creator.AddCreatedKey(this);

        Extent.Add(this);
    }
    
    /*
     * Regional Limited Key constructor
     */
    public LimitedKey(Game game, IAdmin creator, string accessKey, decimal originalPrice, DateTime createdAt, string country, List<string> benefits, decimal limitedPriceIncrease)
        : base(game, creator, accessKey, originalPrice, createdAt, country)
    {
        LimitedPriceIncrease = limitedPriceIncrease;
        Benefits = benefits;
        
        Game.AddKey(this);
        Creator.AddCreatedKey(this);

        Extent.Add(this);
    }

    public override decimal GetCurrentPrice()
    {
        return base.GetCurrentPrice() + LimitedPriceIncrease;
    }
}