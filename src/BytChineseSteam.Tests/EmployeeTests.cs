using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Tests;

public class EmployeeTests
{
    private static readonly string Path = "store.json";

    [SetUp]
    public void Setup()
    {
        File.WriteAllText(Path, "{}");
        ExtentPersistence.LoadAll();
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsNull()
    {
        Assert.Throws<ValidationException>(() =>
        {
            var employee = new Employee(new User(null!, "email@gmail.com", "1234567899", "12345678"), 5000);
        });
    }

    [Test]
    public void ShouldThrowValidationException_WhenEmailIsNull()
    {
        Assert.Throws<ValidationException>(() =>
        {
            var employee = new Employee(new User(new Name { FirstName = "name", LastName = "surname" }, null!,
                "1234567899", "12345678"), 1);
        });
    }

    [Test]
    public void ShouldThrowValidationException_WhenEmailIsInvalid()
    {
        Assert.Throws<ValidationException>(() =>
        {
            var employee = new Employee(new User(new Name { FirstName = "name", LastName = "surname" }, "hello",
                "1234567899", "12345678"), 1);
        });
    }

    [Test]
    public void ShouldThrowValidationException_WhenPhoneNumberIsNull()
    {
        Assert.Throws<ValidationException>(() =>
        {
            var employee = new Employee(new User(new Name { FirstName = "name", LastName = "surname" }, "email@google.com",
                null!, "12345678"), 1);
        });
    }

    [Test]
    public void ShouldThrowValidationException_WhenPhoneNumberInvalid()
    {
        Assert.Throws<ValidationException>(() =>
        {
            var employee = new Employee(new User(new Name { FirstName = "name", LastName = "surname" }, "email@gmail.com",
                "0", "12345678"), 1);
        });
    }

    [Test]
    public void ShouldThrowValidationException_WhenPasswordIsNull()
    {
        Assert.Throws<ValidationException>(() =>
        {
            var employee = new Employee(new User(new Name { FirstName = "name", LastName = "surname" }, "email@gmail.com",
                "1234567899", null!), 1);
        });
    }

    [Test]
    public void ShouldThrowValidationException_WhenPasswordIsInvalid()
    {
        Assert.Throws<ValidationException>(() =>
        {
            var employee = new Employee(new User(new Name { FirstName = "name", LastName = "surname" }, "email@gmail.com",
                "1234567899", "12333"), 1);
        });
    }

    [Test]
    public void ShouldThrowValidationException_WhenSalaryNegative()
    {
        Assert.Throws<ValidationException>(() =>
        {
            var employee = new Employee(new User(new Name { FirstName = "name", LastName = "surname" }, "email@gmail.com",
                "1234567899", "12345678"), -1);
        });
    }

    [Test]
    public void ShouldCreate_WhenPassesValidation()
    {
        Assert.DoesNotThrow(() =>
        {
            var employee = new Employee(new User(new Name { FirstName = "name", LastName = "surname" }, "email@gmail.com",
                "1234567899", "12345678"), 1);
        });
    }
    
    [Test]
    public void ShouldCreate_WhenPassesValidationAndSalaryNull()
    {
        Assert.DoesNotThrow(() =>
        {
            var employee = new Employee(new User(new Name { FirstName = "name", LastName = "surname" }, "email@gmail.com",
                "1234567899", "12345678"), null);
        });
    }
}