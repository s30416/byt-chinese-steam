using BytChineseSteam.Models;
using BytChineseSteam.Models.Enums;
using NUnit.Framework;

namespace BytChineseSteam.Tests;

public class PromotionKeyAssociationTest
{
    private Publisher _publisher;
    private Game _game;
    private Key _key1;
    private Key _key2;
    private Promotion _promotion;

    [SetUp]
    public void Setup()
    {
        var admin = new Admin(new Name("first", "last"), "admin@gmail.com",  "+48123456789", "password", null);
        _publisher = new Publisher("name", "desc", admin);
        _game = new Game("title", "desc", null, _publisher, new Admin(new Name("Big", "Tommy"), 
            "big.tommy@example.com", "+48123456789", "howdoesourhashedpasswork", null));
        
        _key1 = new Key(_game, "key1", 10, DateTime.Now, 0, ["benefit"]);
        _key2 = new Key(_game, "key2", 10, DateTime.Now, 0, ["benefit"]);
        
        // requires an initial Key
        _promotion = new Promotion("promo", 10, DateTime.Now, DateTime.Now, 
            PromotionStatus.Planned, _key1, new Manager(new Name("Small", "Jimmy"), "smallDjim@example.com",
                "+48987654321", "whatwasthehashedpasswordformatagain", 5));
    }

    // Helper to verify state hasn't changed after an exception or invalid action
    public void CheckStateSameAsAfterSetup()
    {
        Assert.That(_promotion.Keys, Is.EquivalentTo(new HashSet<Key> { _key1 }));
        Assert.That(_key1.Promotions, Is.EquivalentTo(new HashSet<Promotion> { _promotion }));
        Assert.That(_key2.Promotions, Is.Empty);
    }

    [Test]
    public void ShouldHaveReverseConnection_OnKeyAddPromotion()
    {
        _key2.AddPromotion(_promotion);
        
        Assert.That(_promotion.Keys, Is.EquivalentTo(new HashSet<Key> { _key1, _key2 }));
        Assert.That(_key2.Promotions, Is.EquivalentTo(new HashSet<Promotion> { _promotion }));
    }

    [Test]
    public void ShouldHaveReverseConnection_OnPromotionAddKey()
    {
        _promotion.AddKey(_key2);
        
        Assert.That(_promotion.Keys, Is.EquivalentTo(new HashSet<Key> { _key1, _key2 }));
        Assert.That(_key2.Promotions, Is.EquivalentTo(new HashSet<Promotion> { _promotion }));
    }

    [Test]
    public void ShouldHaveReverseConnection_OnPromotionRemoveKey()
    {
        _promotion.AddKey(_key2);
        
        _promotion.RemoveKey(_key2);
        
        Assert.That(_promotion.Keys, Is.EquivalentTo(new HashSet<Key> { _key1 }));
        Assert.That(_key2.Promotions, Is.Empty);
    }

    [Test]
    public void ShouldHaveReverseConnection_OnKeyRemovePromotion()
    {
        _promotion.AddKey(_key2);
        
        _key2.RemovePromotion(_promotion);
        
        Assert.That(_promotion.Keys, Is.EquivalentTo(new HashSet<Key> { _key1 }));
        Assert.That(_key2.Promotions, Is.Empty);
    }

    [Test]
    public void ShouldKeepState_OnAddDuplicateKey()
    {
        _promotion.AddKey(_key1);
        CheckStateSameAsAfterSetup();
    }

    [Test]
    public void ShouldThrowArgumentNullException_WhenPromotionIsNull_OnKeyAddPromotion()
    {
        Assert.Throws<ArgumentNullException>(() => _key1.AddPromotion(null!));
    }

    [Test]
    public void ShouldThrowArgumentNullException_WhenKeyIsNull_OnPromotionAddKey()
    {
        Assert.Throws<ArgumentNullException>(() => _promotion.AddKey(null!));
    }

    [Test]
    public void ShouldThrowArgumentNullException_WhenPromotionIsNull_OnKeyRemovePromotion()
    {
        Assert.Throws<ArgumentNullException>(() => _key1.RemovePromotion(null!));
    }

    [Test]
    public void ShouldThrowArgumentNullException_WhenKeyIsNull_OnPromotionRemoveKey()
    {
        Assert.Throws<ArgumentNullException>(() => _promotion.RemoveKey(null!));
    }

    [Test]
    public void ShouldThrowInvalidOperationExceptionAndKeepState_WhenRemovingLastKey_OnPromotionRemoveKey()
    {
        // Enforce 1..* multiplicity
        Assert.Throws<InvalidOperationException>(() => _promotion.RemoveKey(_key1));
        CheckStateSameAsAfterSetup();
    }

    [Test]
    public void ShouldThrowInvalidOperationExceptionAndKeepState_WhenRemovingLastKey_OnKeyRemovePromotion()
    {
        // Enforce 1..* multiplicity via reverse connection
        Assert.Throws<InvalidOperationException>(() => _key1.RemovePromotion(_promotion));
        CheckStateSameAsAfterSetup();
    }
}