using BytChineseSteam.Models;
using BytChineseSteam.Models.Enums;

namespace BytChineseSteam.Tests;

public class ManagerPromotionAssociationTest
{
    private Publisher _publisher;
    private Admin _admin;
    private Game _game;
    private Key _key;
    
    private Manager _manager1;
    private Manager _manager2;
    private Promotion _promotion;

    [SetUp]
    public void Setup()
    {
        _admin = new Admin(new Name("Admin", "User"), "admin@byt.com", "48123456789", "passdddddd", 1000);
        _publisher = new Publisher("PromoPublisher", "Desc", _admin);
        _game = new Game("Promo Game", "Desc", null, _publisher, _admin);
        _key = new Key(_game, "KEY-123", 50, DateTime.Now, 0, new List<string> { "Base Game" });
        
        _manager1 = new Manager(new Name("Boss", "One"), "m1@byt.com", "+48123456789", "passworddddd", 8000);
        _manager2 = new Manager(new Name("Boss", "Two"), "m2@byt.com", "+48123456789", "passworddd", 9000);


        _promotion = new Promotion(
            "Summer Sale", 
            20.0, 
            DateTime.Now, 
            DateTime.Now.AddDays(7), 
            PromotionStatus.Planned,
            _key, 
            _manager1
        );
    }
    
    private void CheckLinkedToManager1()
    {
        Assert.That(_promotion.Manager, Is.EqualTo(_manager1));
        Assert.That(_manager1.Promotions, Contains.Item(_promotion));
        Assert.That(_manager2.Promotions, Does.Not.Contain(_promotion));
    }

    [Test]
    public void ShouldEstablishReverseConnection_OnPromotionConstructor()
    {
        Assert.That(_promotion.Manager, Is.EqualTo(_manager1));
        
        Assert.That(_manager1.Promotions, Contains.Item(_promotion));
        Assert.That(_manager1.Promotions.Count, Is.EqualTo(1));
    }

    [Test]
    public void ShouldThrowArgumentNullException_WhenManagerIsNull_OnConstructor()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new Promotion("Invalid Promo", 10, DateTime.Now, DateTime.Now, PromotionStatus.Planned, _key, null!)
        );
    }

    [Test]
    public void ShouldUpdateReverseConnections_OnChangeManager()
    {
        _promotion.ChangeManager(_manager2);
        
        Assert.That(_promotion.Manager, Is.EqualTo(_manager2));
        
        Assert.That(_manager1.Promotions, Does.Not.Contain(_promotion));
        
        Assert.That(_manager2.Promotions, Contains.Item(_promotion));
    }

    [Test]
    public void ShouldThrowArgumentNullException_WhenNewManagerIsNull_OnChangeManager()
    {
        Assert.Throws<ArgumentNullException>(() => _promotion.ChangeManager(null!));
        
        CheckLinkedToManager1();
    }

    [Test]
    public void ShouldKeepState_WhenChangingToSameManager()
    {
        _promotion.ChangeManager(_manager1);
        
        CheckLinkedToManager1();
        Assert.That(_manager1.Promotions.Count, Is.EqualTo(1));
    }

    [Test]
    public void ShouldAllowMultiplePromotionsForSingleManager()
    {
        var promotion2 = new Promotion(
            "Winter Sale", 
            50.0, 
            DateTime.Now, 
            DateTime.Now.AddDays(7), 
            PromotionStatus.Planned, 
            _key, 
            _manager1
        );
        
        Assert.That(_manager1.Promotions.Count, Is.EqualTo(2));
        Assert.That(_manager1.Promotions, Contains.Item(_promotion));
        Assert.That(_manager1.Promotions, Contains.Item(promotion2));
        
        Assert.That(_promotion.Manager, Is.EqualTo(_manager1));
        Assert.That(promotion2.Manager, Is.EqualTo(_manager1));
    }
}