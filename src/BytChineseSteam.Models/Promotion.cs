namespace BytChineseSteam.Models;

public enum PromotionStatus
{
    Planned,
    Completed,
    Finished,
    ToBeDeleted
}

public class Promotion
{
    public string Name { get; set; }
    public double DiscountPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PromotionStatus Status { get; set; }
}