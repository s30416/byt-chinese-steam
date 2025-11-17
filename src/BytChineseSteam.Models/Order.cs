using System.Runtime.CompilerServices;
using BytChineseSteam.Models.Enums;

namespace BytChineseSteam.Models;



public class Order
{
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public double TotalSum { get; set; }
}