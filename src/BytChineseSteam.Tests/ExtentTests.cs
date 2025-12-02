using System.Text.Json;
using System.Text.Json.Nodes;
using BytChineseSteam.Models;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Tests;

public class ExtentTest
{
    private const string Path = "store.json";

    [Test]
    public void ShouldBeEmpty_OnStart()
    {
        Assert.That(Key.Extent.All(), Is.Empty);
    }

    [Test]
    public void ShouldHaveEntity_AfterConstruction()
    {
        var key = new Key("asdf", 10, DateTime.Now, 0, []);
        Assert.That(Key.Extent.All(), Is.EquivalentTo(new [] { key }));
    }
}