using System.Text.Json.Serialization;
using BytChineseSteam.Repository.Extent;
namespace BytChineseSteam.Models;

public class Customer : User
{
    private static readonly Extent<Customer> Extent = new();
    private readonly HashSet<Order> _orders = new();
    
    [JsonIgnore]
    public IReadOnlyCollection<Order> Orders => _orders.ToList().AsReadOnly();

    public Customer(Name name, string email, string phoneNumber, string hashedPassword) 
        : base(name, email, phoneNumber, hashedPassword)
    {
        Extent.Add(this);
    }

    public static IReadOnlyList<Customer> ViewAllCustomers()
    {
        return Extent.All();
    }
    
    // association with order
    internal void AddOrder(Order order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));
        
        if (!_orders.Contains(order))
        {
            _orders.Add(order);
        }
    }

    internal void RemoveOrder(Order order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));
        
        _orders.Remove(order);
    }
}