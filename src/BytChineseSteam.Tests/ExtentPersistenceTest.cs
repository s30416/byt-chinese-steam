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
    
    [Test]
    public void EntityPersistence_ShouldPersistExtents()
    {
        // storing key
        var key = new Key("asdf", 10, DateTime.Now, 0, []);
        ExtentPersistence.Persist(Key.Extent);
        
        // checking json
        var text = File.ReadAllText(Path);
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
}