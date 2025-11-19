using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BytChineseSteam.Models.DataAnnotations;

namespace BytChineseSteam.Models;

public class Key : Limited
{
    // fields
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
    
    public required List<string> Benefits { get; set; }
    
    // extent collection
    private static List<Key> _keys = new();

    [JsonConstructor]
    private Key()
    {
        // extent
        AddKey(this);
    }
    
    // extent methods
    public static ReadOnlyCollection<Key> ViewAllKeys()
    {
        return _keys.AsReadOnly();
    }

    private static void AddKey(Key key)
    {
        if (key == null)
        {
            throw new ArgumentException("Given key cannot be null");
        }
        
        _keys.Add(key);
    }
    
    // class methods from diagram
    // ...
}