using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using BytChineseSteam.Models.DataAnnotations;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public class Key : ExtentModel<Key>, Limited
{
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

    [MinLength(1)] [Required] public string AccessKey { get; set; }

    [NonNegative] [Required] public decimal OriginalPrice { get; set; }

    [DateNotInFuture] [Required] public DateTime CreatedAt { get; set; }

    [NonNegative] [Required] public decimal PriceIncrease { get; set; }

    [NonEmpty(isArray: true)] [Required] public List<string> Benefits { get; set; }

    // class methods from diagram
    // ...

    public decimal GetCurrentPrice()
    {
        return OriginalPrice + PriceIncrease;
    }
}