//TODO:
//currently tests work only when run separate - aka code is fked
//I tried to fix but failed, problem is that Order cannot be empty
//it's 3am and I declare defeat - I hope that @Ram_er or whoever the fuck elps
//please

using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models;

namespace BytChineseSteam.Tests;

[TestFixture]
[TestOf(typeof(Key))]
public class KeyTest
{
    private Game CreateValidGame()
    {
        var publisher = new Publisher("Test Publisher", "Description");
        var admin = new Admin(new Name("Big", "Tommy"), "big.tommy@example.com", "+48123456789", "howdoesourhashedpasswork", null);
        return new Game("Test Game", "Description", publisher, admin);
    }

    [SetUp]
    public void Setup()
    {
        var allGames = Game.ViewAllGames.ToList();
        foreach (var game in allGames)
        {
            game.DeleteGame();
        }
        
    }

    [Test]
    public void Creation_ShouldReturnCorrectData()
    {
        var game = CreateValidGame();
        var now = DateTime.Now;
        var key = new Key(game, "some key", 10, now, 0, []);

        Assert.Multiple(() =>
        {
            Assert.That(key.Game, Is.EqualTo(game));
            Assert.That(key.AccessKey, Is.EqualTo("some key"));
            Assert.That(key.OriginalPrice, Is.EqualTo(10));
            Assert.That(key.CreatedAt, Is.EqualTo(now));
            Assert.That(key.PriceIncrease, Is.EqualTo(0));
            Assert.That(key.Benefits, Is.Empty);
        });
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenAccessKeyIsEmptyString()
    {
        var game = CreateValidGame();
        Assert.Throws<ValidationException>(() => new Key(game, "", 10, DateTime.Now, 0, []));
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenOriginalPriceIsNegative()
    {
        var game = CreateValidGame();
        Assert.Throws<ValidationException>(() => new Key(game, "some key", -1, DateTime.Now, 0, []));
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenCreatedAtIsInFuture()
    {
        var game = CreateValidGame();
        Assert.Throws<ValidationException>(() => new Key(game, "123", 10, DateTime.Now.AddDays(1), 0, []));
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenPriceIncreaseNegative()
    {
        var game = CreateValidGame();
        Assert.Throws<ValidationException>(() => new Key(game, "some key", 10, DateTime.Now, -1, []));
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenBenefitsContainEmptyString()
    {
        var game = CreateValidGame();
        Assert.Throws<ValidationException>(() => new Key(game, "some key", 10, DateTime.Now, 0, [""]));
    }
    
    [Test]
    public void Validation_ShouldThrowArgumentNull_WhenGameIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new Key(null!, "some key", 10, DateTime.Now, 0, []));
        Assert.That(ex!.ParamName, Is.EqualTo("game"));
    }
}