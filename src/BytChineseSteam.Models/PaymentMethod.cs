using System.Collections.ObjectModel;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;


public class PaymentMethod
{
    private static readonly Extent<PaymentMethod> Extent = new();
    
    public string Name { get; set; }

    private HashSet<Order> _orders = new();
    public bool ContainsOrder(Order order) => _orders.Contains(order);
    public ReadOnlyCollection<Order> GetOrders() => _orders.ToList().AsReadOnly(); 
    
    public PaymentMethod(string name)
    {
        Name = name;
        
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