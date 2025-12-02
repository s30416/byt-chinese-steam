using System.Text.Json;
using System.Text.Json.Nodes;
using BytChineseSteam.Models;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Tests;

public class ExtentPersistenceTest
{
    private const string Path = "store.json";

    [SetUp]
    public void Setup()
    {
        File.WriteAllText(Path, "{}");
        ExtentPersistence.LoadAll();
        
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
    
    [Test]
    public void EntityPersistence_ShouldPersistExtents()
    {
        // Creating dependencies
        var publisher = new Publisher("Persistence Pub", "Test Description");
        var game = new Game("Persistence Game", "Test Description", publisher);

        // storing key
        var key = new Key(game, "asdf", 10, DateTime.Now, 0, []);
        ExtentPersistence.Persist(Key.Extent);
    
        // checking json
        var text = File.ReadAllText("store.json"); 
        var extentJson = JsonSerializer.SerializeToNode(Key.Extent.All());
        var json = JsonNode.Parse(text)![Key.Extent.Name]!;
    
        Assert.That(JsonNode.DeepEquals(json, extentJson), Is.True);
    }

    [Test]
    public void ShouldDiscoverExtentsAndLoadModels_WhenDiscoverExtentsAndLoadAllIsCalled()
    {
        // writing to file
     var root = JsonNode.Parse("{}")!;
        var keyNodeArray = JsonSerializer.SerializeToNode<List<object>>([
            new
            {
                AccessKey = "adsf",
                OriginalPrice = 10,
                CreatedAt = DateTime.Now,
                PriceIncrease = 0,
                Benefits = new string[]{},
            }
        ]);
    
        root[Key.Extent.Name] = keyNodeArray;
        File.WriteAllText(Path, root.ToJsonString());
        
        // checking
        ExtentPersistence.DiscoverExtents();
        ExtentPersistence.LoadAll();
        var retrievedNode = JsonSerializer.SerializeToNode(Key.Extent.All());
        
        Assert.That(JsonNode.DeepEquals(retrievedNode, keyNodeArray), Is.True);
    }
    
    [Test]
    public void ShouldThrowException_WhenRegisteringTheSameExtent()
    {
        ExtentPersistence.DiscoverExtents();
        Assert.That(() => ExtentPersistence.Register(Key.Extent), Throws.ArgumentException);
    }
    
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
        var publisher = new Publisher("Test Publisher", "Test Description");
        var game = new Game("Test Game", "Test Description", publisher);

        var key = new Key(game, "adsf", 0, DateTime.Now, 0, []);
        Assert.That(new List<Key> { key }, Is.EquivalentTo(Key.Extent.All()));
    }

    [Test, Order(5)]
    public void Extent_ShouldHaveValuesLoadedByPersistence_IfContainsExtent()
    {
        // filling store.js
        var json =
            $"{{\"Key\":[{{\"AccessKey\":null,\"OriginalPrice\":0,\"CurrentPrice\":0,\"CreatedAt\":\"2025-11-19T14:20:13.608448+01:00\",\"PriceIncrease\":0,\"Benefits\":[]}}]}}";

        File.WriteAllText(Path, json);

        // loading values
        ExtentPersistence.LoadAll();
        Assert.That(Key.Extent.All().Count, Is.GreaterThanOrEqualTo(1));
    }

    [Test, Order(6)]
    public void Extent_HasCorrectObject_WhenLoadingPersistentExtent()
    {
        // filling store.js
        var jsonArrNode = JsonSerializer.SerializeToNode((List<object>)
            [
                new
                {
                    AccessKey = "asdf",
                    OriginalPrice = 10,
                    CreatedAt = DateTime.Now,
                    PriceIncrease = 0,
                    Benefits = new List<string>(),
                }
            ]
        );
        var json = $"{{\"Key\":{jsonArrNode!.ToJsonString()}}}";
    
        File.WriteAllText(Path, json);
    
        // loading values
        ExtentPersistence.LoadAll();
    
        var retrievedJsonNode = JsonSerializer.SerializeToNode(Key.Extent.All()) as JsonArray;
    
        Console.WriteLine(jsonArrNode);
        Console.WriteLine(retrievedJsonNode);
    
        Assert.That(JsonNode.DeepEquals(retrievedJsonNode, jsonArrNode), Is.True);
    }
}