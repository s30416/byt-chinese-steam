using BytChineseSteam.Models.Enums;

namespace BytChineseSteam.Models;

public class Promotion
{
    public string Name { get; set; }
    public double DiscountPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PromotionStatus Status { get; set; }
}