using System.Text.Json;
using BytChineseSteam.Models;
using BytChineseSteam.Repository.Extent;

// File.Delete("store.json");
ExtentPersistence.LoadAll();

Console.WriteLine(Key.Extent.All().Count);
foreach (var k in Key.Extent.All())
{
    Console.WriteLine(JsonSerializer.Serialize(k));
}

// var game = new Game("asdf", "asdf", null, new Publisher("adsf", "adsf"));
// var key = new Key(game, "asdf", 1, DateTime.Now, 0, []);

// var key = Key.Extent.All().First();
// key.AccessKey = "134234";
// var values = Key.Extent.All();
// Console.WriteLine(JsonSerializer.Serialize(Key.Extent.All()));

ExtentPersistence.Persist(Key.Extent);
Console.WriteLine(File.ReadAllText("store.json"));