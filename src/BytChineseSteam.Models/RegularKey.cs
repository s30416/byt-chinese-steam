using BytChineseSteam.Models.Interfaces;

namespace BytChineseSteam.Models;

public class RegularKey : Key
{
    public static int ReturnWindow { get; } = 7;
    
    /*
     * Universal Regular Key constructor
     */
    public RegularKey(Game game, IAdmin creator, string accessKey, decimal originalPrice, DateTime createdAt, decimal universalPriceIncrease) 
        : base(game, creator, accessKey, originalPrice, createdAt, universalPriceIncrease)
    {
        Game.AddKey(this);
        Creator.AddCreatedKey(this);

        Extent.Add(this);
    }

    /*
     * Regional Regular Key constructor
     */
    public RegularKey(Game game, IAdmin creator, string accessKey, decimal originalPrice, DateTime createdAt, string country)
        : base(game, creator, accessKey, originalPrice, createdAt, country)
    {
        Game.AddKey(this);
        Creator.AddCreatedKey(this);

        Extent.Add(this);
    }

}