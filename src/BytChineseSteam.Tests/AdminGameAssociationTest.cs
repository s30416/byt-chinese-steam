using BytChineseSteam.Models;

namespace BytChineseSteam.Tests;

public class AdminGameAssociationTest
{
    private Publisher _publisher;
    private Admin _admin1;
    private Admin _admin2;
    private Game _game;

    [SetUp]
    public void Setup()
    {
        var name1 = new Name("John", "Doe"); 
        var name2 = new Name("Jane", "Smith");

        var user1 = new User(name1, "admin1@byt.com", "+48123456789", "hashed_pass");
        var user2 = new User(name2, "admin2@byt.com", "+48123456789", "hashed_pass");

        _admin1 = new Admin(user1, 5000);
        _admin2 = new Admin(user2, 5000);
        
        _publisher = new Publisher("BytPublisher", "Best publisher", _admin1);
        
        _game = new Game("Super Game", "Description", null, _publisher, _admin1);
    }
    
    private void CheckLinkedToAdmin1()
    {
        Assert.That(_game.Admin, Is.EqualTo(_admin1));
        Assert.That(_admin1.Games, Contains.Item(_game));
        Assert.That(_admin2.Games, Does.Not.Contain(_game));
    }

    [Test]
    public void ShouldEstablishReverseConnection_OnGameConstructor()
    {
        Assert.That(_game.Admin, Is.EqualTo(_admin1));
        
        Assert.That(_admin1.Games, Contains.Item(_game));
        Assert.That(_admin1.Games.Count, Is.EqualTo(1));
    }

    [Test]
    public void ShouldThrowArgumentNullException_WhenAdminIsNull_OnConstructor()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new Game("Invalid Game", "Desc", null, _publisher, null!)
        );
    }

    [Test]
    public void ShouldUpdateReverseConnections_OnChangeAdmin()
    {
        _game.ChangeAdmin(_admin2);
        
        Assert.That(_game.Admin, Is.EqualTo(_admin2));
        
        Assert.That(_admin1.Games, Does.Not.Contain(_game));
        
        Assert.That(_admin2.Games, Contains.Item(_game));
    }

    [Test]
    public void ShouldThrowArgumentNullException_WhenNewAdminIsNull_OnChangeAdmin()
    {
        Assert.Throws<ArgumentNullException>(() => _game.ChangeAdmin(null!));
        
        CheckLinkedToAdmin1();
    }

    [Test]
    public void ShouldKeepState_WhenChangingToSameAdmin()
    {
        _game.ChangeAdmin(_admin1);
        
        CheckLinkedToAdmin1();
        Assert.That(_admin1.Games.Count, Is.EqualTo(1));
    }

    [Test]
    public void ShouldAllowMultipleGamesForSingleAdmin()
    {
        var game2 = new Game("Second Game", "Desc", null, _publisher, _admin1);
        
        Assert.That(_admin1.Games.Count, Is.EqualTo(2));
        Assert.That(_admin1.Games, Contains.Item(_game));
        Assert.That(_admin1.Games, Contains.Item(game2));
        
        Assert.That(_game.Admin, Is.EqualTo(_admin1));
        Assert.That(game2.Admin, Is.EqualTo(_admin1));
    }

    [Test]
    public void ShouldRemoveFromAdmin_OnDeleteGame()
    {
        _game.DeleteGame();
        
        Assert.That(_admin1.Games, Does.Not.Contain(_game));
        Assert.That(_admin1.Games, Is.Empty);
    }
}