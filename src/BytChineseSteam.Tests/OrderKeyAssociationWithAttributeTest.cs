using BytChineseSteam.Models;
using BytChineseSteam.Models.Enums;
using BytChineseSteam.Models.Exceptions.OrderKey;

namespace BytChineseSteam.Tests;

public class OrderKeyAssociationWithAttributeTest
{
    private Game _game;
    private Order _order;
    private Key _key;
    private Key _key2;
    private Publisher _publisher;
    private OrderKey _orderKey1;
    private OrderKey _orderKey2;
    private Admin _adminRole;
    
    [SetUp]
    public void Setup()
    {
        var adminEmp = new Employee(
            new Name("first", "last"), 
            "admin@gmail.com", 
            "+48123456789", 
            "password", 
            null
        );
        _adminRole = new Admin(adminEmp);
        
        _publisher = new Publisher("name", "desc", _adminRole);
        
        var gameAdminEmp = new Employee(
            new Name("Big", "Tommy"), 
            "big.tommy@example.com", 
            "+48123456789", 
            "howdoesourhashedpasswork", 
            null
        );
        var gameAdminRole = new Admin(gameAdminEmp);

        _game = new Game("title", "desc", _publisher, gameAdminRole);
        
        
        _key = new RegularKey(_game, _adminRole, "asdf", 10, DateTime.Now, 0);
        _key2 = new RegularKey(_game, _adminRole, "asdf", 10, DateTime.Now, 0);        
        
        _order = new Order(DateTime.Now, OrderStatus.Active, DateTime.Now, 0, new HashSet<Key>() {_key}, new Customer(new Name("Lil", "Bomba"), "bigBOOM@its3am.here",
            "+54341242532", "istilldontknowhashedpassformat"));
        
        _orderKey1 = new OrderKey(_order, _key);
        _orderKey2 = new OrderKey(_order, _key2);
    }

    // 
    public void CheckStateSameAsAfterSetup()
    {
        Assert.That(_order.Keys, Is.EquivalentTo(new HashSet<OrderKey> { _orderKey1 }));
        Assert.That(_key.Orders, Is.EquivalentTo(new HashSet<OrderKey> { _orderKey1 }));
    }
    
    [Test]
    public void ShouldHaveReverseConnection_OnKeyAddToOrder()
    {
        _key2.AddToOrder(_order);
        Assert.That(_order.Keys, Is.EquivalentTo(new HashSet<OrderKey> { _orderKey1, _orderKey2 }));
        Assert.That(_key2.Orders, Is.EquivalentTo(new HashSet<OrderKey> { _orderKey2 }));
    }
    
    [Test]
    public void ShouldHaveReverseConnection_OnOrderAddKey()
    {
        _order.AddKey(_key2);
        Assert.That(_order.Keys, Is.EquivalentTo(new HashSet<OrderKey> { _orderKey1, _orderKey2 }));
        Assert.That(_key2.Orders, Is.EquivalentTo(new HashSet<OrderKey> { _orderKey2 }));
    }

    [Test]
    public void ShouldHaveReverseConnection_OnOrderRemoveKey()
    {
        _order.AddKey(_key2);
        
        _order.RemoveKey(_key2);
        Assert.That(_order.Keys, Is.EquivalentTo(new HashSet<OrderKey>() { _orderKey1 }));
        Assert.That(_key2.Orders, Is.EquivalentTo(new HashSet<OrderKey>() {  }));
    }
    
    [Test]
    public void ShouldHaveReverseConnection_OnKeyRemoveFromOrder()
    {
        _order.AddKey(_key2);
        
        _order.RemoveKey(_key2);
        Assert.That(_order.Keys, Is.EquivalentTo(new HashSet<OrderKey>() { _orderKey1 }));
        Assert.That(_key2.Orders, Is.EquivalentTo(new HashSet<OrderKey>() {  }));
    }

    [Test]
    public void ShouldThrowKeyExistsInOrderExceptionAndKeepState_OnKeyAddToOrder()
    {
        Assert.Throws<KeyExistsInOrderException>( () => _key.AddToOrder(_order) );
        CheckStateSameAsAfterSetup();
    }
    
    [Test]
    public void ShouldThrowKeyExistsInOrderExceptionAndKeepState_OnOrderAddKey()
    {
        Assert.Throws<KeyExistsInOrderException>( () => _order.AddKey(_key) );
        CheckStateSameAsAfterSetup();
    }

    [Test]
    public void ShouldThrowArgumentNullException_WhenOrderIsNull_OnKeyAddToOrder()
    {
        Assert.Throws<ArgumentNullException>( () => _key.AddToOrder(null!) );
    }
    
    [Test]
    public void ShouldThrowArgumentNullException_WhenKeyIsNull_OnOrderAddKey()
    {
        Assert.Throws<ArgumentNullException>( () => _order.AddKey(null!) );
    }
    
    [Test]
    public void ShouldThrowKeyDoesNotExistInOrderExceptionAndKeepState_OnKeyRemoveFromOrder()
    {
        Assert.Throws<KeyDoesNotExistInOrderException>(() => _key2.RemoveFromOrder(_order));
        CheckStateSameAsAfterSetup();
    }
    
    [Test]
    public void ShouldThrowKeyDoesNotExistInOrderExceptionAndKeepState_OnOrderRemoveKey()
    {
        Assert.Throws<KeyDoesNotExistInOrderException>(() => _order.RemoveKey(_key2));
        CheckStateSameAsAfterSetup();
    }

    [Test]
    public void ShouldThrowArgumentNullException_WhenOrderIsNull_OnKeyRemoveFromOrder()
    {
        Assert.Throws<ArgumentNullException>( () => _key.RemoveFromOrder(null!) );
    }
    
    [Test]
    public void ShouldThrowArgumentNullException_WhenKeyIsNull_OnOrderRemoveKey()
    {
        Assert.Throws<ArgumentNullException>( () => _order.RemoveKey(null!) );
    }

    [Test]
    public void ShouldAddKeysToOrders_WhenPassedToConstructor_OnOrderCreation()
    {
        var k3 = new RegularKey(_game, _adminRole, "asdf", 10, DateTime.Now, 0);
        var k4 = new RegularKey(_game, _adminRole, "asdf", 10, DateTime.Now, 0);
        var k5 = new RegularKey(_game, _adminRole, "asdf", 10, DateTime.Now, 0);
        
        var order = new Order( DateTime.Now, OrderStatus.Active, DateTime.Now, 0, [k3, k4, k5],
            new Customer(new Name("Lil", "Bomba"), "bigBOOM@its3am.here", "+54341242532", "istilldontknowhashedpassformat"));

        var ok3 = new OrderKey(order, k3);
        var ok4 = new OrderKey(order, k4);
        var ok5 = new OrderKey(order, k5);
        
        Assert.That(order.Keys, Is.EquivalentTo(new[] { ok3, ok4, ok5 }));
        Assert.That(k3.Orders, Is.EquivalentTo(new[] { ok3 }));
        Assert.That(k4.Orders, Is.EquivalentTo(new[] { ok4 }));
        Assert.That(k5.Orders, Is.EquivalentTo(new[] { ok5 }));
    }
}