using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Tests;

public class KeyRegionalUniversalLimitedRegular_MultiAspectInheritanceTest
{
    // cant set country to non regional
    // cant set price increase to non universal
    // null exceptions
    // correct price

    private Category _category;
    private Game _game;
    private Publisher _publisher;
    private Admin _admin;
    
    [SetUp]
    public void Setup()
    {
        _admin = new Admin(new Name("first", "last"), "admin@mail.com", "1234567899", "passwor123", 0, null);
        _publisher = new Publisher("publisher", "description", _admin);
        _category = new Category("category");
        _game = new Game("game", "", _category, _publisher, _admin);
        
    }

    private void Validate(Object element)
    {
        var context = new ValidationContext(element);
        Validator.ValidateObject(element, context, true);
    }

    [Test]
    public void ShouldThrowArgumentException_WhenSettingCountry_ForNonRegionalLimitedKey()
    {
        var key = new LimitedKey(_game, _admin, "123", 10, DateTime.Now, 10, [], 1);
        Assert.Throws<ArgumentException>(() => key.Country = "asdf");
    }
    
    [Test]
    public void ShouldThrowArgumentException_WhenSettingCountry_ForNonRegionalRegularKey()
    {
        var key = new RegularKey(_game, _admin, "123", 10, DateTime.Now, 10);
        Assert.Throws<ArgumentException>(() => key.Country = "asdf");
    }
    
    [Test]
    public void ShouldThrowArgumentException_WhenSettingUniversalPriceIncrease_ForNonUniversalLimitedKey()
    {
        var key = new LimitedKey(_game, _admin, "123", 10, DateTime.Now, "country", [], 1);
        Assert.Throws<ArgumentException>(() => key.UniversalPriceIncrease = 11);
    }
    
    [Test]
    public void ShouldThrowArgumentException_WhenSettingUniversalPriceIncrease_ForNonUniversalRegularKey()
    {
        var key = new RegularKey(_game, _admin, "123", 10, DateTime.Now, "country");
        Assert.Throws<ArgumentException>(() => key.UniversalPriceIncrease = 11);
    }

    [Test]
    public void ShouldThrowNullArgumentException_WhenSettingUniversalPriceIncreaseToNull_ForUniversalKey()
    {
        var key = new LimitedKey(_game, _admin, "123", 10, DateTime.Now, 1, [], 1);
        Assert.Throws<ArgumentNullException>(() => key.UniversalPriceIncrease = null);
    }
    
    [Test]
    public void ShouldReturnCorrectPrice_ForUniversalLimitedKey()
    {
        var key = new LimitedKey(_game, _admin, "123", 10, DateTime.Now, 1, [], 1);
        Assert.That(key.GetCurrentPrice(), Is.EqualTo(12m));
    }
    
    [Test]
    public void ShouldReturnCorrectPrice_ForUniversalRegularKey()
    {
        var key = new RegularKey(_game, _admin, "123", 10, DateTime.Now, 1);
        Assert.That(key.GetCurrentPrice(), Is.EqualTo(11m));
    }

    [Test]
    public void ShouldReturnCorrectPrice_ForRegionalRegularKey()
    {
        var key = new RegularKey(_game, _admin, "123", 10, DateTime.Now, "country");
        Assert.That(key.GetCurrentPrice(), Is.EqualTo(10m));
    }
    
    [Test]
    public void ShouldReturnCorrectPrice_ForRegionalLimitedKey()
    {
        var key = new LimitedKey(_game, _admin, "123", 10, DateTime.Now, "country", [], 1);
        Assert.That(key.GetCurrentPrice(), Is.EqualTo(11m));
    }

    [Test]
    public void ShouldThrowValidationException_WhenSettingUniversalPriceIncrease_ToNegative()
    {
        var key = new LimitedKey(_game, _admin, "123", 10, DateTime.Now, 1, [], 1);
        
        key.UniversalPriceIncrease = -1;
        Assert.Throws<ValidationException>( () => Validate(key));
    }
    
    [Test]
    public void ShouldThrowValidationException_WhenSettingLimitedPriceIncrease_ToNegative()
    {
        var key = new LimitedKey(_game, _admin, "123", 10, DateTime.Now, 1, [], 1);
        key.LimitedPriceIncrease = -1;
            
        Assert.Throws<ValidationException>( () => Validate(key));
    }
}