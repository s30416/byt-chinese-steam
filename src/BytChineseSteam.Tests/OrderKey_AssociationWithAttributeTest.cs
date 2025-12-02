using BytChineseSteam.Models;
using BytChineseSteam.Models.Enums;

namespace BytChineseSteam.Tests;

public class OrderKeyAssociationWithAttributeTest
{
    private Game _game;
    private Order _order;
    private Key _key;
    private Publisher _publisher;
    private OrderKey _sampleOrderKey;
    
    [SetUp]
    public void Setup()
    {
        _publisher = new Publisher("name", "desc");
        _game = new Game("title", "desc", null, _publisher);
        _key = new(_game, "asdf", 10, DateTime.Now, 0, []);
        _order = new Order(DateTime.Now, OrderStatus.Active, DateTime.Now, 0, new HashSet<Key>() {_key});
        _sampleOrderKey = new OrderKey(_order, _key);
    }
    
    [Test]
    public void ShouldHaveEmptySets_OnStart()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_key.Orders.Count, Is.EqualTo(0));
            Assert.That(_order.Keys.Count, Is.EqualTo(0));
        });
    }

    [Test]
    public void ShouldHaveReverseConnection_OnKeyAddToOrder()
    {
        _key.AddToOrder(_order);
        Assert.That(_order.Keys, Is.EquivalentTo(new HashSet<OrderKey> { _sampleOrderKey }));
        Assert.That(_key.Orders, Is.EquivalentTo(new HashSet<OrderKey> { _sampleOrderKey }));
    }
    
    [Test]
    public void ShouldHaveReverseConnection_OnOrderAddKey()
    {
        _order.AddKey(_key);
        Assert.That(_order.Keys, Is.EquivalentTo(new HashSet<OrderKey> { _sampleOrderKey }));
        Assert.That(_key.Orders, Is.EquivalentTo(new HashSet<OrderKey> { _sampleOrderKey }));
    }

    [Test]
    public void ShouldHaveReverseConnection_OnOrderRemoveKey()
    {
        _order.AddKey(_key);
        
        _order.RemoveKey(_key);
        Assert.That(_order.Keys.Count, Is.EqualTo(0));
        Assert.That(_key.Orders.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void ShouldHaveReverseConnection_OnKeyRemoveFromOrder()
    {
        _order.AddKey(_key);
        
        _key.RemoveFromOrder(_order);
        Assert.That(_order.Keys.Count, Is.EqualTo(0));
        Assert.That(_key.Orders.Count, Is.EqualTo(0));
    }
}