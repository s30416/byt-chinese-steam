using System.Text.Json;
using System.Text.Json.Nodes;
using BytChineseSteam.Models;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Tests;

public class ExtentTest
{
    // private const string Path = "store.json";

    // [Test]
    // public void ShouldBeEmpty_OnStart()
    // {
    //     Assert.That(Key.Extent.All(), Is.Empty);
    // }

    // I CHANGE THIS TEST WITH ONE BELOW CAUSE OF COMPOSITION
    
    // [Test]
    // public void ShouldHaveEntity_AfterConstruction()
    // {
    //     var key = new Key("asdf", 10, DateTime.Now, 0, []);
    //     Assert.That(Key.Extent.All(), Is.EquivalentTo(new [] { key }));
    // }
    
    [Test]
    public void ShouldHaveEntity_AfterConstruction()
    {
        var admin = new Admin(new Name("Big", "Tommy"), "big.tommy@example.com", "+48123456789", "howdoesourhashedpasswork", null);
        var publisher = new Publisher("Test Publisher", "Test Description", admin);
        var game = new Game("Test Game", "Test Description", publisher, admin);

        var key = new RegularKey(game, admin, "asdf", 10, DateTime.Now, 0);
        Assert.That(Key.Extent.All(), Is.EquivalentTo(new [] { key }));
    }
}