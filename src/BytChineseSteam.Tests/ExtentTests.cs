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

    [Test, Order(4)]
    public void Extent_ShouldHaveValues_AfterModelCreation()
    {
        var key = new Key()
        {
            AccessKey = "asdf",
            Benefits = [],
            CreatedAt = DateTime.Now,
            OriginalPrice = 10,
            PriceIncrease = 0
        };
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