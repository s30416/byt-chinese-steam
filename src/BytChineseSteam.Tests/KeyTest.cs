using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models;

namespace BytChineseSteam.Tests;

[TestFixture]
[TestOf(typeof(Key))]
public class KeyTest
{
    private static void ValidateModel(object model)
    {
        var context = new ValidationContext(model);
    
        // Manually invokes all validation attributes
        Validator.ValidateObject(model, context, true);
    }
    
    [Test]
    public void Creation_ShouldReturnCorrectData()
    {
        var now = DateTime.Now;
        var key = new Key
        {
            AccessKey = "some key",
            OriginalPrice = 10,
            CreatedAt = now,
            PriceIncrease = 0,
            Benefits = []
        };
        
        ValidateModel(key);
        
        Assert.Multiple(() =>
        {
            Assert.That(key.AccessKey, Is.EqualTo("some key"));
            Assert.That(key.OriginalPrice, Is.EqualTo(10));
            Assert.That(key.CreatedAt, Is.EqualTo(now));
            Assert.That(key.PriceIncrease, Is.EqualTo(0));
            Assert.That(key.Benefits, Is.EqualTo(new string[]{} ));
        });
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenAccessKeyIsEmptyString()
    {
        var key = new Key
        {
            AccessKey = "",
            OriginalPrice = 10,
            CreatedAt = DateTime.Now,
            PriceIncrease = 0,
            Benefits = []
        };
        
        Assert.Throws<ValidationException>(() =>  ValidateModel(key));
    }
    
    [Test]
    public void Validation_ShouldThrowValidationError_WhenOriginalPriceIsNegative()
    {
        var key = new Key
        {
            AccessKey = "adsf",
            OriginalPrice = -1,
            CreatedAt = DateTime.Now,
            PriceIncrease = 0,
            Benefits = []
        };
        
        Assert.Throws<ValidationException>(() =>  ValidateModel(key));
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenCreatedAtIsInFuture()
    {
        var key = new Key
        {
            AccessKey = "",
            OriginalPrice = 10,
            CreatedAt = DateTime.Now.AddDays(1),
            PriceIncrease = 0,
            Benefits = []
        };
        
        Assert.Throws<ValidationException>(() =>  ValidateModel(key));
    }
    
    [Test]
    public void Validation_ShouldThrowValidationError_WhenPriceIncreaseNegative()
    {
        var key = new Key
        {
            AccessKey = "",
            OriginalPrice = 10,
            CreatedAt = DateTime.Now,
            PriceIncrease = -1,
            Benefits = []
        };
        
        Assert.Throws<ValidationException>(() =>  ValidateModel(key));
    }

    
}