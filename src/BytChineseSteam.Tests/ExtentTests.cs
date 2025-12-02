using System.Text.Json;
using System.Text.Json.Nodes;
using BytChineseSteam.Models;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Tests;

public class ExtentTest
{
    private const string Path = "store.json";

    [SetUp]
    public void ClearJsonFile()
    {
        File.WriteAllText(Path, "{}");
        ExtentPersistence.LoadAll();
    }

    [Test]
    public void ShouldBeEmpty_OnStart()
    {
        Assert.That(Key.Extent.All(), Is.Empty);
    }

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
        var publisher = new Publisher("Test Publisher", "Test Description");
        var game = new Game("Test Game", "Test Description", publisher);

        var key = new Key(game, "asdf", 10, DateTime.Now, 0, []);
        Assert.That(Key.Extent.All(), Is.EquivalentTo(new [] { key }));
    }
}