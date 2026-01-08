using BytChineseSteam.Models;
using BytChineseSteam.Models.Enums;
using BytChineseSteam.Models.Interfaces;

namespace BytChineseSteam.Tests;

public class CustomerOrderAssociationTest
{
    private Publisher _publisher;
    private IAdmin _adminRole;
    private Game _game;
    private Key _key;

    private Customer _customer1;
    private Order _order;
    
    private Employee _adminEmployee;

    [SetUp]
    public void Setup()
    {
        _adminEmployee = new Employee(
            new Name("Admin", "User"), 
            "a@b.com", 
            "+48000000000", 
            "passwasfsdd", 
            5000, 
            null
        );
        _adminRole = _adminEmployee.AssignAdminRole();
        
        _publisher = new Publisher("OrderPub", "Desc", _adminRole);
        
        _game = new Game("Order Game", "Desc", null, _publisher, _adminRole);
        
        _key = new RegularKey(_game, _adminRole, "KEY-ORDER-TEST", 100, DateTime.Now, 0);        
        
        _customer1 = new Customer(new Name("John", "Buyer"), "c1@b.com", "+48111111111", "passwssaaa");
        
        _order = new Order(
            DateTime.Now, 
            OrderStatus.Planned, 
            null,
            100.0, 
            new List<Key> { _key }, 
            _customer1
        );
    }

    [Test]
    public void ShouldEstablishReverseConnection_OnOrderConstructor()
    {
        Assert.That(_order.Customer, Is.EqualTo(_customer1));
        
        Assert.That(_customer1.Orders, Contains.Item(_order));
        Assert.That(_customer1.Orders.Count, Is.EqualTo(1));
    }

    [Test]
    public void ShouldThrowArgumentNullException_WhenCustomerIsNull_OnConstructor()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new Order(DateTime.Now, OrderStatus.Planned, null, 100, new List<Key> { _key }, null!)
        );
    }

    [Test]
    public void ShouldAllowMultipleOrdersForSingleCustomer()
    {
        var key2 = new RegularKey(_game, _adminRole, "KEY-ORDER-TEST-2", 100, DateTime.Now, 0);
        
        var order2 = new Order(
            DateTime.Now, 
            OrderStatus.Finished, 
            DateTime.Now, 
            200.0, 
            new List<Key> { key2 }, 
            _customer1
        );
        
        Assert.That(_customer1.Orders.Count, Is.EqualTo(2));
        Assert.That(_customer1.Orders, Contains.Item(_order));
        Assert.That(_customer1.Orders, Contains.Item(order2));
        
        Assert.That(_order.Customer, Is.EqualTo(_customer1));
        Assert.That(order2.Customer, Is.EqualTo(_customer1));
    }
}