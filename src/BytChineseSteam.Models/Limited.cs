namespace BytChineseSteam.Models;

public interface Limited
{
    public static int ReturnWindow { get; } = 14;
    
    public decimal PriceIncrease { get; set; }
    
    public List<string> Benefits { get; set; }
}