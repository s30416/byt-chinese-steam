using BytChineseSteam.Models;

namespace BytChineseSteam.Tests;

public class AdminKeyAssociationTests
{
    private Admin _admin;
    private Game _game;
    private Publisher _publisher;

    [SetUp]
    public void Setup()
    {
        _admin = new Admin(
            new Name("Key", "Test"), 
            "keytest@steam.com", 
            "+48123456789", 
            "hashdgdfgdfg", 
            5000, 
            creator: null
        );

        _publisher = new Publisher("Test Pub", "Desc", _admin);

        _game = new Game("Test Game", "Desc", _publisher, _admin);
    }

    [Test]
    public void ShouldEstablishReverseConnection_OnKeyCreation()
    {
        var key = new Key(_game, _admin, "KEY-123", 100, DateTime.Now, 0, new List<string> { "Benefit" });

        Assert.That(key.Creator, Is.EqualTo(_admin));

        Assert.That(_admin.CreatedKeys, Does.Contain(key));
        Assert.That(_admin.CreatedKeys.Count, Is.EqualTo(1));
    }

    [Test]
    public void ShouldRemoveFromAdminList_OnKeyDeletion()
    {
        var key = new Key(_game, _admin, "KEY-DEL", 100, DateTime.Now, 0, new List<string> { "Benefit" });
        
        Assert.That(_admin.CreatedKeys, Does.Contain(key));

        key.DeleteKey();

        Assert.That(_admin.CreatedKeys, Does.Not.Contain(key));
        
        Assert.That(Key.Extent.All(), Does.Not.Contain(key));
    }

    [Test]
    public void ShouldThrowException_WhenCreatingKeyWithNullAdmin()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => 
            new Key(_game, null!, "KEY-NULL", 100, DateTime.Now, 0, new List<string>())
        );

        Assert.That(ex!.ParamName, Is.EqualTo("creator"));
    }

    [Test]
    public void ShouldThrowException_WhenAddingKeyCreatedByOtherAdmin()
    {
        var otherAdmin = new Admin(new Name("Other", "Guy"), "other@b.com", "+48123456789", "hashddfgdfg", 1000, null);
        
        var key = new Key(_game, _admin, "KEY-OWNED", 100, DateTime.Now, 0, new List<string>());

        var ex = Assert.Throws<InvalidOperationException>(() => 
            otherAdmin.AddCreatedKey(key)
        );

        Assert.That(ex!.Message, Does.Contain("Key creator mismatch"));
    }
    
    [Test]
    public void ShouldAllowAdminToCreateMultipleKeys()
    {
        var key1 = new Key(_game, _admin, "KEY-1", 10, DateTime.Now, 0, new List<string>());
        var key2 = new Key(_game, _admin, "KEY-2", 20, DateTime.Now, 0, new List<string>());

        Assert.That(_admin.CreatedKeys.Count, Is.EqualTo(2));
        Assert.That(_admin.CreatedKeys, Does.Contain(key1));
        Assert.That(_admin.CreatedKeys, Does.Contain(key2));
    }
}