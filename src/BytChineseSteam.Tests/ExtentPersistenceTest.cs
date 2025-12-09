using System.Text.Json;
using System.Text.Json.Nodes;
using BytChineseSteam.Models;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Tests;

[NonParallelizable]
public class ExtentPersistenceTest
{
    private const string Path = "store.json";
    private Game _game = new Game("Test Game", "Test Description", null!, new Admin(new Name("Big", "Tommy"), 
        "big.tommy@example.com", "+48123456789", "howdoesourhashedpasswork", null));
    

    [SetUp]
    public void Setup()
    {
        File.WriteAllText(Path, "{}");
        ExtentPersistence.LoadAll();

        while (true)
        {
            var key = Key.Extent.All().FirstOrDefault();

            if (key == null) break;
            
            Key.Extent.Remove(key);
        }
    }
    
    // I CHANGE THIS TEST WITH ONE BELOW CAUSE OF COMPOSITION
    
    // [Test]
    // public void EntityPersistence_ShouldPersistExtents()
    // {
    //     // storing key
    //     var key = new Key("asdf", 10, DateTime.Now, 0, []);
    //     ExtentPersistence.Persist(Key.Extent);
    //     
    //     // checking json
    //     var text = File.ReadAllText(Path);
    //     var extentJson = JsonSerializer.SerializeToNode(Key.Extent.All());
    //     var json = JsonNode.Parse(text)![Key.Extent.Name]!;
    //     
    //     Assert.That(JsonNode.DeepEquals(json, extentJson), Is.True);
    // }
    
    [Test, Order(1)]
    public void Extent_ShouldBeEmptyList_OnStart()
    {
        Assert.That(new List<Key>(), Is.EquivalentTo(Key.Extent.All()));
    }

    [Test, Order(2)]
    public void Extent_ShouldBeEmptyList_OnLoadingNonExistingFile()
    {
        ExtentPersistence.LoadAll();
        Assert.That(new List<Key>(), Is.EquivalentTo(Key.Extent.All()));
    }

    [Test, Order(3)]
    public void Extent_ShouldBeEmptyList_OnPersistingEmptyExtent()
    {
        ExtentPersistence.LoadAll();
        ExtentPersistence.Persist(Key.Extent);
        Assert.That(new List<Key>(), Is.EquivalentTo(Key.Extent.All()));
    }

    // I CHANGE THIS TEST WITH ONE BELOW CAUSE OF COMPOSITION
    
    // [Test, Order(4)]
    // public void Extent_ShouldHaveValues_AfterModelCreation()
    // {
    //     var key = new Key("adsf", 0, DateTime.Now, 0, []);
    //     Assert.That(new List<Key> { key }, Is.EquivalentTo(Key.Extent.All()));
    // }
    
    [Test, Order(4)]
    public void Extent_ShouldHaveValues_AfterModelCreation()
    {
        // Creating dependencies
        var admin = new Admin(new Name("Big", "Tommy"), "big.tommy@example.com", "+48123456789", "howdoesourhashedpasswork", null);
        var publisher = new Publisher("Test Publisher", "Test Description", admin);
        var game = new Game("Test Game", "Test Description", publisher, admin);

        var key = new Key(game, "adsf", 0, DateTime.Now, 0, []);
        Assert.That(new List<Key> { key }, Is.EquivalentTo(Key.Extent.All()));
    }

    [Test, Order(5)]
    public void Extent_ShouldHaveValuesLoadedByPersistence_IfContainsExtent()
    {
        // filling store.js
        var key = new Key(_game, "adsf", 0, DateTime.Now, 0, []);
        ExtentPersistence.Persist(Key.Extent);

        // loading values
        ExtentPersistence.LoadAll();
        Assert.That(Key.Extent.All().Count, Is.GreaterThanOrEqualTo(1));
    }
    
    [Test, Order(5)]
    public void EntityPersistence_ShouldPersistExtents()
    {
        // Creating dependencies
        var admin = new Admin(new Name("Big", "Tommy"), "big.tommy@example.com", "+48123456789", "howdoesourhashedpasswork", null);
        var publisher = new Publisher("Persistence Pub", "Test Description", admin);
        var game = new Game("Persistence Game", "Test Description", publisher, admin);

        // storing key
        var key = new Key(game, "asdf", 10, DateTime.Now, 0, []);
        ExtentPersistence.Persist(Key.Extent);
    
        // checking json
        var text = File.ReadAllText("store.json"); 
        var extentJson = JsonSerializer.SerializeToNode(Key.Extent.All());
        var json = JsonNode.Parse(text)![Key.Extent.Name]!;
    
        Assert.That(JsonNode.DeepEquals(json, extentJson), Is.True);
    }

    [Test, Order(6)]
    public void ShouldDiscoverExtentsAndLoadModels_WhenDiscoverExtentsAndLoadAllIsCalled()
    {
        // writing to file
        var key = new Key(_game, "asdf", 10, DateTime.Now, 0, []);
        var keyNodeArray = JsonSerializer.SerializeToNode(new List<Key>() {key});
        Console.WriteLine(Key.Extent.All().Count());
        ExtentPersistence.Persist(Key.Extent);
        
        // checking
        ExtentPersistence.DiscoverExtents();
        ExtentPersistence.LoadAll();
        var retrievedNode = JsonSerializer.SerializeToNode(Key.Extent.All());
        
        Console.WriteLine(keyNodeArray.ToJsonString());
        Console.WriteLine(retrievedNode.ToJsonString());
        
        Assert.That(JsonNode.DeepEquals(retrievedNode, keyNodeArray), Is.True);
    }
    
    [Test, Order(7)]
    public void ShouldThrowException_WhenRegisteringTheSameExtent()
    {
        ExtentPersistence.DiscoverExtents();
        Assert.That(() => ExtentPersistence.Register(Key.Extent), Throws.ArgumentException);
    }
}