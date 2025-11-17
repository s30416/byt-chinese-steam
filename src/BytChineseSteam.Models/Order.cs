using System.Runtime.CompilerServices;

namespace BytChineseSteam.Models;

public enum OrderStatus
{
    Planned,
    Active,
    Finished,
    ToBeDeleted
}

public class Order
{
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public double TotalSum { get; set; }
}