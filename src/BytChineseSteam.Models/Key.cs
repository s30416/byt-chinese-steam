using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models.DataAnnotations;

namespace BytChineseSteam.Models;

public class Key : Limited
{
    [MinLength(1)]
    public required string AccessKey { get; set; }
    
    [NonNegative]
    public required decimal OriginalPrice  { get; set; }
    
    public decimal CurrentPrice => OriginalPrice + PriceIncrease;
    
    [DateNotInFuture]
    [Required]
    public required DateTime CreatedAt { get; set; }
    
    [NonNegative]
    public required decimal PriceIncrease { get; set; }
    
    public required string[] Benefits { get; set; }
}