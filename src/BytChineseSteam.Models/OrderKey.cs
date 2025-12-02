namespace BytChineseSteam.Models;

public class OrderKey(Order order, Key key)
{
    public DateTime AddedAt { get; } = DateTime.Now;
    public Order Order { get; } = order;
    public Key Key {get; } = key;

    public override bool Equals(object? obj)
    {
        if (obj is not OrderKey key) return false;
        
        return key.Key == Key && key.Order == Order; 
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Key, Order);
    }
}