//TODO:
//currently tests work only when run separate - aka code is fked
//I tried to fix but failed, problem is that Order cannot be empty
//it's 3am and I declare defeat - I hope that @Ram_er or whoever the fuck elps
//please

using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models;
using BytChineseSteam.Models.Interfaces;

namespace BytChineseSteam.Tests;

[TestFixture]
[TestOf(typeof(Key))]
public class KeyTest
{
    private (Game, IAdmin) CreateDependencies()
    {
        var adminEmployee = new Employee(
            new Name("Big", "Tommy"), 
            "big.tommy@example.com", 
            "+48123456789", 
            "hashpasswordforfun", 
            null
        );
        
        var adminRole = adminEmployee.AssignAdminRole();
        
        var publisher = new Publisher("Test Publisher", "Description", adminRole);
        
        var game = new Game("Test Game", "Description", publisher, adminRole);
        
        return (game, adminRole);
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
        var (game, admin) = CreateDependencies();
        var now = DateTime.Now;
        var key = new RegularKey(game, admin, "some key", 10, now, 0);

        Assert.Multiple(() =>
        {
            Assert.That(key.Game, Is.EqualTo(game));
            Assert.That(key.Creator, Is.EqualTo(admin));
            Assert.That(key.AccessKey, Is.EqualTo("some key"));
            Assert.That(key.OriginalPrice, Is.EqualTo(10));
            Assert.That(key.CreatedAt, Is.EqualTo(now));
            Assert.That(key.UniversalPriceIncrease, Is.EqualTo(0));
        });
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenAccessKeyIsEmptyString()
    {
        var (game, admin) = CreateDependencies();
        Assert.Throws<ValidationException>(() => new RegularKey(game, admin, "", 10, DateTime.Now, 0));
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenOriginalPriceIsNegative()
    {
        var (game, admin) = CreateDependencies();
        Assert.Throws<ValidationException>(() => new RegularKey(game, admin, "some key", -1, DateTime.Now, 0));
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenCreatedAtIsInFuture()
    {
        var (game, admin) = CreateDependencies();
        Assert.Throws<ValidationException>(() => new RegularKey(game, admin, "123", 10, DateTime.Now.AddDays(1), 0));
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenPriceIncreaseNegative()
    {
        var (game, admin) = CreateDependencies();
        Assert.Throws<ValidationException>(() => new RegularKey(game, admin, "some key", 10, DateTime.Now, -1));
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenBenefitsContainEmptyString()
    {
        var (game, admin) = CreateDependencies();
        Assert.Throws<ValidationException>(() => new LimitedKey(game, admin, "some key", 10, DateTime.Now, 0, new List<string> { "" }, 0));
    }
    
    [Test]
    public void Validation_ShouldThrowArgumentNull_WhenGameIsNull()
    {
        var (_, admin) = CreateDependencies();
        
        var ex = Assert.Throws<ArgumentNullException>(() => new RegularKey(null!, admin, "some key", 10, DateTime.Now, 0));
        Assert.That(ex!.ParamName, Is.EqualTo("game"));
    }
    
    [Test]
    public void Validation_ShouldThrowArgumentNull_WhenAdminIsNull()
    {
        var (game, _) = CreateDependencies();
        
        var ex = Assert.Throws<ArgumentNullException>(() => new RegularKey(game, null!, "some key", 10, DateTime.Now, 0));
        Assert.That(ex!.ParamName, Is.EqualTo("creator"));
    }
}