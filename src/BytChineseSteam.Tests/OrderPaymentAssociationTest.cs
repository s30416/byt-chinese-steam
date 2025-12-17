using BytChineseSteam.Models;
using BytChineseSteam.Models.Enums;

namespace BytChineseSteam.Tests;

public class OrderPaymentAssociationTest
{
    private Customer _customer;
    private PaymentMethod _pmA;
    private PaymentMethod _pmB;
    private Key _key1;
    private Key _key2;
    private Order _order1;
    private Order _order2;

    [SetUp]
    public void Setup()
    {
        var admin = new Admin(new Name("Admin", "User"), "admin@test.local", "+48123456789", "passssss", null);
        var game = new Game("TestGame", "desc", new Publisher("PubX", "desc", admin), admin);

        _customer = new Customer(new Name("Lil", "Bomba"), "test@customer.local", "+48123455555", "passssss");

        _pmA = new PaymentMethod("A");
        _pmB = new PaymentMethod("B");

        _key1 = new RegularKey(game, admin, "AAA", 10, DateTime.Now, 0);
        _key2 = new RegularKey(game, admin, "BBB", 10, DateTime.Now, 0);

        _order1 = new Order(
            DateTime.Now,
            OrderStatus.Active,
            null,
            10,
            new HashSet<Key> { _key1 },
            _customer,
            _pmA
        );

        _order2 = new Order(
            DateTime.Now,
            OrderStatus.Active,
            null,
            20,
            new HashSet<Key> { _key2 },
            _customer,
            _pmA
        );
    }

    [Test]
    public void PaymentMethod_ShouldContainOrdersAddedInConstructor()
    {
        Assert.That(_pmA.GetOrders(), Does.Contain(_order1));
        Assert.That(_pmA.GetOrders(), Does.Contain(_order2));
    }

    [Test]
    public void AddPaymentMethod_ShouldUpdateBothSides()
    {
        var admin = new Admin(new Name("Mini", "Admin"), "new@a.b", "+48000000000", "passaaaa", null);
        var game = new Game("ExtraGame", "desc", new Publisher("PubY", "desc", admin), admin);
        var key = new RegularKey(game, admin, "KEY", 10, DateTime.Now, 0);

        var customer = new Customer(new Name("User", "N"), "x@y.z", "+48123123123", "passssss");

        var order = new Order(
            DateTime.Now,
            OrderStatus.Active,
            null,
            10,
            new HashSet<Key> { key },
            customer,
            _pmB
        );

        Assert.That(_pmB.GetOrders(), Does.Contain(order));
        Assert.That(order.PaymentMethod, Is.EqualTo(_pmB));
    }

    [Test]
    public void AddPaymentMethod_Idempotent()
    {
        _order1.AddPaymentMethod(_pmA);
        _order1.AddPaymentMethod(_pmA);

        Assert.That(_pmA.GetOrders().Count(o => o == _order1), Is.EqualTo(1));
        Assert.That(_order1.PaymentMethod, Is.EqualTo(_pmA));
    }

    [Test]
    public void ChangePaymentMethod_ShouldMoveOrderBetweenPaymentMethods()
    {
        _order1.AddPaymentMethod(_pmB);

        Assert.That(_order1.PaymentMethod, Is.EqualTo(_pmB));
        Assert.That(_pmB.GetOrders(), Does.Contain(_order1));
        Assert.That(_pmA.GetOrders(), Does.Not.Contain(_order1));
    }

    [Test]
    public void ChangePaymentMethod_Idempotent()
    {
        _order1.AddPaymentMethod(_pmA);
        _order1.AddPaymentMethod(_pmA);

        Assert.That(_order1.PaymentMethod, Is.EqualTo(_pmA));
        Assert.That(_pmA.GetOrders().Count(o => o == _order1), Is.EqualTo(1));
    }

    [Test]
    public void RemovePaymentMethod_ShouldUpdateBothSides()
    {
        _order1.RemovePaymentMethod();

        Assert.That(_order1.PaymentMethod, Is.Null);
        Assert.That(_pmA.GetOrders(), Does.Not.Contain(_order1));
    }

    [Test]
    public void RemovePaymentMethod_Idempotent()
    {
        _order1.RemovePaymentMethod();
        _order1.RemovePaymentMethod();

        Assert.That(_order1.PaymentMethod, Is.Null);
        Assert.That(_pmA.GetOrders(), Does.Not.Contain(_order1));
    }

    [Test]
    public void RemoveOrderFromPaymentMethod_ShouldBreakReverseLink()
    {
        _pmA.RemoveOrder(_order1);

        Assert.That(_pmA.GetOrders(), Does.Not.Contain(_order1));
        Assert.That(_order1.PaymentMethod, Is.Null);
    }

    [Test]
    public void RemoveOrder_Idempotent()
    {
        _pmA.RemoveOrder(_order1);
        _pmA.RemoveOrder(_order1);

        Assert.That(_pmA.GetOrders(), Does.Not.Contain(_order1));
        Assert.That(_order1.PaymentMethod, Is.Null);
    }

    [Test]
    public void MultipleOrders_MoveOne_DoesNotAffectOthers()
    {
        _order1.AddPaymentMethod(_pmB);

        Assert.That(_order1.PaymentMethod, Is.EqualTo(_pmB));
        Assert.That(_order2.PaymentMethod, Is.EqualTo(_pmA));

        Assert.That(_pmB.GetOrders(), Does.Contain(_order1));
        Assert.That(_pmA.GetOrders(), Does.Contain(_order2));
    }

    [Test]
    public void SettingPaymentMethodToNull_RemovesOrderOnlyFromOriginal()
    {
        _order1.RemovePaymentMethod();

        Assert.That(_pmA.GetOrders(), Does.Not.Contain(_order1));
        Assert.That(_order1.PaymentMethod, Is.Null);
    }
}
