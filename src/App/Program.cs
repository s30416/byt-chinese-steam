using BytChineseSteam.Models;

ExtentPersistence.LoadAll();

// var k = new Key
// {
//     AccessKey = null,
//     OriginalPrice = 0,
//     CreatedAt = default,
//     PriceIncrease = 0,
//     Benefits = []
// };
// Key.Extent.Add(k);
// ExtentPersistence.Persist(Key.Extent);

foreach (var key in Key.Extent.All())
{
    Console.WriteLine(key);
}
