using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BytChineseSteam.Repository.Extent;

public class Extent<T> : IExtent
{
    private List<T> _items = [];
    public string Name => typeof(T).Name;

    public Extent()
    {
        ExtentPersistence.Register(this);
    }

    public void Add(T item)
    {
        if (item == null)
        {
            throw new ArgumentException("Item cannot be null");
        }

        // can call validation here

        _items.Add(item);
    }

    public ReadOnlyCollection<T> All()
    {
        return _items.AsReadOnly();
    }

    public void LoadFromJsonArray(JsonArray element)
    {
        try
        {
            var items = element.Deserialize<List<T>>();
            _items = items ?? throw new ArgumentException($"Received null for {Name} items");
        }
        catch (Exception e)
        {
            if (e is JsonException je || e is NotSupportedException ne)
                throw new Exception($"Error loading items for {Name}. Invalid value in JSON. Message: {e.Message}");
        }
    }

    public JsonNode ToJson()
    {
        return JsonSerializer.SerializeToNode(All())!;
    }
}