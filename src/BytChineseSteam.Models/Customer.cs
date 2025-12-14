using System.Text.Json.Serialization;
using BytChineseSteam.Repository.Extent;
namespace BytChineseSteam.Models;

public class Customer
{
    private static readonly Extent<Customer> Extent = new();
    private readonly HashSet<Order> _orders = new();

    [JsonIgnore] public IReadOnlyCollection<Order> Orders => _orders.ToList().AsReadOnly();
    
    // inheritance
    [JsonIgnore]
    public User User { get; private set; }

    // literally default built-in inheritance with extra steps...
    public Customer(User user)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        user.AddCustomer(this); // reverse connection
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

    public void ChangeUser(User newUser)
    {
        throw new InvalidOperationException("Customer cannot change its User, since it's inheritance.");
    }
    
    public static void DeleteCustomer(Customer customer)
    {
        if  (customer == null) 
            throw new ArgumentNullException(nameof(customer), "Customer cannot be null.");
        
        Extent.Remove(customer);
        customer.User.RemoveCustomer(customer);
    }
}