using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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

    private static void Validate(T element)
    {
        if (element == null)
        {
            throw new ArgumentException($"Element cannot be null");
        }
        
        var context = new ValidationContext(element);
        Validator.ValidateObject(element, context, true);
    }
    public void Add(T item)
    {
        Validate(item);
        _items.Add(item);
    }

    public void Remove(T item)
    {
        _items.Remove(item);
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

            if (items == null) return;
            
            foreach (var item in items)
            {
                Validate(item);
            }

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
        foreach (var item in All())
        {
            Validate(item);
        }
        return JsonSerializer.SerializeToNode(All())!;
    }
}