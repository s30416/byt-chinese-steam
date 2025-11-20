using System.Text.Json;
using BytChineseSteam.Models;
using BytChineseSteam.Repository.Extent;

ExtentPersistence.LoadAll();

Console.WriteLine(Key.Extent.All().Count);
foreach (var k in Key.Extent.All())
{
    Console.WriteLine(JsonSerializer.Serialize(k));
}

// var key = new Key("asdf", 1, DateTime.Now, 0, []);

var key = Key.Extent.All().First();
key.AccessKey = "134234";
var values = Key.Extent.All();
Console.WriteLine(JsonSerializer.Serialize(Key.Extent.All()));

// ExtentPersistence.Persist(Key.Extent);
// Key.Extent.Update();