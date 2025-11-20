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
        var key = new Key("some key", 10, now, 0, []);

        Assert.Multiple(() =>
        {
            Assert.That(key.AccessKey, Is.EqualTo("some key"));
            Assert.That(key.OriginalPrice, Is.EqualTo(10));
            Assert.That(key.CreatedAt, Is.EqualTo(now));
            Assert.That(key.PriceIncrease, Is.EqualTo(0));
            Assert.That(key.Benefits, Is.EqualTo(new string[] { }));
        });
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenAccessKeyIsEmptyString()
    {
        Assert.Throws<ValidationException>(() => new Key("", 10, DateTime.Now, 0, []));
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenOriginalPriceIsNegative()
    {
        Assert.Throws<ValidationException>(() => new Key("some key", -1, DateTime.Now, 0, []));
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenCreatedAtIsInFuture()
    {
        Assert.Throws<ValidationException>(() => new Key("123", 10, DateTime.Now.AddDays(1), 0, []));
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenPriceIncreaseNegative()
    {
        Assert.Throws<ValidationException>(() => new Key("", 10, DateTime.Now, -1, []));
    }

    [Test]
    public void Validation_ShouldThrowValidationError_WhenBenefitsEmptyString()
    {
        Assert.Throws<ValidationException>(() => new Key("some key", 10, DateTime.Now, 0, [""]));
    }
}