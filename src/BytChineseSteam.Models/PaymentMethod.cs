using System.Collections.ObjectModel;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public abstract class PaymentMethod
{
    private static readonly Extent<PaymentMethod> Extent = new();
    
    public string Name { get; set; }

    private HashSet<Order> _orders = new();
    
    // Association methods remain shared
    public bool ContainsOrder(Order order) => _orders.Contains(order);
    public ReadOnlyCollection<Order> GetOrders() => _orders.ToList().AsReadOnly(); 
    
    // Protected constructor: only subclasses can call this
    protected PaymentMethod(string name)
    {
        Name = name;
        
        // Adds the specific subclass instance (e.g., GooglePay) to the common Extent
        Extent.Add(this);
    }
    
    public void AddOrder(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (_orders.Contains(order)) return;

        _orders.Add(order);

        if (order.PaymentMethod != this)
            order.AddPaymentMethod(this);
    }

    public void RemoveOrder(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (!_orders.Contains(order)) return;

        _orders.Remove(order);

        if (order.PaymentMethod == this)
            order.RemovePaymentMethod();
    }
    
    public static ReadOnlyCollection<PaymentMethod> GetAll()
    {
        return Extent.All();
    }
}