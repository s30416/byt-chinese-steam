using System.Text.Json;
using BytChineseSteam.Models;
using BytChineseSteam.Repository.Extent;

ExtentPersistence.LoadAll();

var k = new Key
{
    AccessKey = null,
    OriginalPrice = 0,
    CreatedAt = DateTime.Now,
    PriceIncrease = 0,
    Benefits = [],
};
Key.Extent.Add(k);
// ExtentPersistence.Persist(Key.Extent);

foreach (var key in Key.Extent.All())
{
    Console.WriteLine(JsonSerializer.Serialize(key));
}
