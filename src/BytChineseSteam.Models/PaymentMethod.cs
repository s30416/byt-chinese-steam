using System.Collections.ObjectModel;

namespace BytChineseSteam.Models;

// TODO: ask if we need an Extent for this class

public class PaymentMethod
{
    public string Name { get; set; }

    private HashSet<Order> _orders = new();
    public bool ContainsOrder(Order order) => _orders.Contains(order);
    public ReadOnlyCollection<Order> GetOrders() => _orders.ToList().AsReadOnly(); 
    
    public PaymentMethod(string name)
    {
        Name = name;
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
}