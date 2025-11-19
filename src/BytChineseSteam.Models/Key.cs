using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models.DataAnnotations;

namespace BytChineseSteam.Models;

public class Key : Limited
{
    public static readonly Extent<Key> Extent = new ();
    
    [MinLength(1)]
    public required string AccessKey { get; set; }
    
    [NonNegative]
    public required decimal OriginalPrice  { get; set; }
    
    [DateNotInFuture]
    [Required]
    public required DateTime CreatedAt { get; set; }
    
    [NonNegative]
    public required decimal PriceIncrease { get; set; }
    
    public required string[] Benefits { get; set; }

    public decimal GetCurrentPrice()
    {
        return OriginalPrice + PriceIncrease;
    }
}