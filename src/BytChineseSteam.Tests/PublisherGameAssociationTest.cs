using BytChineseSteam.Models;

namespace BytChineseSteam.Tests;

public class PublisherGameAssociationTest
{
    private Publisher _publisherA;
    private Publisher _publisherB;
    private Game _game1;
    private Game _game2;

    [SetUp]
    public void Setup()
    {
        var dummyAdmin = new Admin(new User(new Name("A", "A"), "a@a.a", "+48000000000", "passaaaa"), null);
        
        _publisherA = new Publisher("PubA", "DescA", dummyAdmin);
        _publisherB = new Publisher("PubB", "DescB", dummyAdmin);

        _game1 = new Game("Game One", "Desc1", _publisherA, dummyAdmin);
        _game2 = new Game("Game Two", "Desc2", _publisherA, dummyAdmin);
    }

    [Test]
    public void Publisher_ShouldContainGamesAddedInConstructor()
    {
        Assert.That(_publisherA.GetAllPublishersGames, Does.Contain(_game1));
        Assert.That(_publisherA.GetAllPublishersGames, Does.Contain(_game2));
    }

    [Test]
    public void AddGame_ShouldUpdateBothSides()
    {
        var dummyAdmin = new Admin(new User(new Name("B", "B"), "b@b.b", "+48000000000", "passaaaa"), null);

        var game = new Game("Extra Game", "DescX", _publisherB, dummyAdmin);

        Assert.That(_publisherB.GetAllPublishersGames, Does.Contain(game));
        Assert.That(game.Publisher, Is.EqualTo(_publisherB));
    }

    [Test]
    public void AddGame_Idempotent()
    {
        _publisherA.AddGame(_game1);
        _publisherA.AddGame(_game1);

        Assert.That(_publisherA.GetAllPublishersGames().Count, Is.EqualTo(2));
    }

    [Test]
    public void ChangePublisher_ShouldMoveGameBetweenPublishers()
    {
        _game1.ChangePublisher(_publisherB);

        Assert.That(_game1.Publisher, Is.EqualTo(_publisherB));
        Assert.That(_publisherB.GetAllPublishersGames, Does.Contain(_game1));
        Assert.That(_publisherA.GetAllPublishersGames, Does.Not.Contain(_game1));
    }

    [Test]
    public void ChangePublisher_Idempotent()
    {
        _game1.ChangePublisher(_publisherA);
        _game1.ChangePublisher(_publisherA);

        Assert.That(_game1.Publisher, Is.EqualTo(_publisherA));
        Assert.That(_publisherA.GetAllPublishersGames().Count, Is.EqualTo(2));
    }

    [Test]
    public void RemoveGame_ThrowsIfStillAssignedToPublisher()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            _publisherA.RemoveGame(_game1);
        });
    }

    [Test]
    public void RemoveGameAllowed_WhenGameMovedAway()
    {
        _game1.ChangePublisher(_publisherB);

        Assert.DoesNotThrow(() => _publisherA.RemoveGame(_game1));

        Assert.That(_publisherA.GetAllPublishersGames, Does.Not.Contain(_game1));
        Assert.That(_publisherB.GetAllPublishersGames, Does.Contain(_game1));
    }

    [Test]
    public void GameDeletion_RemovesFromPublisher()
    {
        _game1.DeleteGame();

        Assert.That(_publisherA.GetAllPublishersGames, Does.Not.Contain(_game1));
    }
}