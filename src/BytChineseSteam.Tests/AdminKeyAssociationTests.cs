using BytChineseSteam.Models;
using BytChineseSteam.Models.Interfaces;

namespace BytChineseSteam.Tests;

public class AdminKeyAssociationTests
{
    private Employee _adminEmployee;
    private IAdmin _adminRole;
    private Game _game;
    private Publisher _publisher;
    

    [SetUp]
    public void Setup()
    {
        
        _adminEmployee = new Employee(
            new Name("Key", "Test"), 
            "keytest@steam.com", 
            "+48123456789", 
            "hashdgdfgdfg", 
            5000,
            null
        );

        _adminRole = _adminEmployee.AssignAdminRole();

        _publisher = new Publisher("Test Pub Key", "Desc", _adminRole);
        _game = new Game("Test Game Key", "Desc", _publisher, _adminRole);
    }

    [Test]
    public void ShouldEstablishReverseConnection_OnKeyCreation()
    {
        var key = new RegularKey(_game, _adminRole, "KEY-123", 100, DateTime.Now, 0);

        Assert.That(key.Creator, Is.EqualTo(_adminRole));

        Assert.That(_adminRole.CreatedKeys, Does.Contain(key));
        Assert.That(_adminRole.CreatedKeys.Count, Is.EqualTo(1));
    }

    [Test]
    public void ShouldRemoveFromAdminList_OnKeyDeletion()
    {
        var key = new RegularKey(_game, _adminRole, "KEY-DEL", 100, DateTime.Now, 0);
        
        Assert.That(_adminRole.CreatedKeys, Does.Contain(key));

        key.DeleteKey();

        Assert.That(_adminRole.CreatedKeys, Does.Not.Contain(key));
        
        Assert.That(Key.Extent.All(), Does.Not.Contain(key));
    }

    [Test]
    public void ShouldThrowException_WhenCreatingKeyWithNullAdmin()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => 
            new RegularKey(_game, null!, "KEY-NULL", 100, DateTime.Now, 0)
        );

        Assert.That(ex!.ParamName, Is.EqualTo("creator"));
    }

    [Test]
    public void ShouldThrowException_WhenAddingKeyCreatedByOtherAdmin()
    {
        var otherEmp = new Employee(new Name("Other", "Guy"), "other@b.com", "+48123456789", "hashddfgdfg", 5000, null);
        var otherAdminRole = otherEmp.AssignAdminRole();
        
        var key = new RegularKey(_game, _adminRole, "KEY-OWNED", 100, DateTime.Now, 0);

        var ex = Assert.Throws<InvalidOperationException>(() => 
            otherAdminRole.AddCreatedKey(key)
        );

        Assert.That(ex!.Message, Does.Contain("Key creator mismatch"));
    }
    
    [Test]
    public void ShouldAllowAdminToCreateMultipleKeys()
    {
        var key1 = new RegularKey(_game, _adminRole, "KEY-1", 10, DateTime.Now, 0);
        var key2 = new RegularKey(_game, _adminRole, "KEY-2", 20, DateTime.Now, 0);

        Assert.That(_adminRole.CreatedKeys.Count, Is.EqualTo(2));
        Assert.That(_adminRole.CreatedKeys, Does.Contain(key1));
        Assert.That(_adminRole.CreatedKeys, Does.Contain(key2));
    }
}