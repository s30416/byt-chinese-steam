using BytChineseSteam.Models;
using BytChineseSteam.Models.Enums;

namespace BytChineseSteam.Tests;

public class CustomerOrderAssociationTest
{
    private Publisher _publisher;
    private Admin _admin;
    private Game _game;
    private Key _key;

    private Customer _customer1;
    private Order _order;

    [SetUp]
    public void Setup()
    {
        // 1. Dependencies
        _publisher = new Publisher("OrderPub", "Desc");
        _admin = new Admin(new Name("Admin", "User"), "a@b.com", "+48000000000", "passwasfsdd", 1000);
        _game = new Game("Order Game", "Desc", null, _publisher, _admin);
        _key = new Key(_game, "KEY-ORDER-TEST", 100, DateTime.Now, 0, new List<string> { "Benefit" });

        // 2. Customer
        _customer1 = new Customer(new Name("John", "Buyer"), "c1@b.com", "+48111111111", "passwssaaa");

        // 3. Order (Using OrderStatus.Planned)
        _order = new Order(
            DateTime.Now, 
            OrderStatus.Planned, 
            null, // CompletedAt is null for Planned
            100.0, 
            new List<Key> { _key }, 
            _customer1 // <--- Association established here
        );
    }

    [Test]
    public void ShouldEstablishReverseConnection_OnOrderConstructor()
    {
        // Assert: Order points to Customer
        Assert.That(_order.Customer, Is.EqualTo(_customer1));
        
        // Assert: Customer points back to Order
        Assert.That(_customer1.Orders, Contains.Item(_order));
        Assert.That(_customer1.Orders.Count, Is.EqualTo(1));
    }

    [Test]
    public void ShouldThrowArgumentNullException_WhenCustomerIsNull_OnConstructor()
    {
        // Enforce Mandatory 1 constraint
        Assert.Throws<ArgumentNullException>(() => 
            new Order(DateTime.Now, OrderStatus.Planned, null, 100, new List<Key> { _key }, null!)
        );
    }

    [Test]
    public void ShouldAllowMultipleOrdersForSingleCustomer()
    {
        // Arrange
        var key2 = new Key(_game, "KEY-ORDER-TEST-2", 100, DateTime.Now, 0, new List<string> { "Benefit" });

        // Create second order for same customer
        var order2 = new Order(
            DateTime.Now, 
            OrderStatus.Finished, 
            DateTime.Now, 
            200.0, 
            new List<Key> { key2 }, 
            _customer1 // Link to same customer
        );

        // Assert: Customer1 has both orders
        Assert.That(_customer1.Orders.Count, Is.EqualTo(2));
        Assert.That(_customer1.Orders, Contains.Item(_order));
        Assert.That(_customer1.Orders, Contains.Item(order2));
        
        // Assert: Both orders point to Customer1
        Assert.That(_order.Customer, Is.EqualTo(_customer1));
        Assert.That(order2.Customer, Is.EqualTo(_customer1));
    }
}