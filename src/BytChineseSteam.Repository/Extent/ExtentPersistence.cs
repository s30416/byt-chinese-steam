using System.Reflection;
using System.Text.Json.Nodes;

namespace BytChineseSteam.Repository.Extent;

public abstract class ExtentPersistence
{
    private const string Path = "store.json";
    private static readonly List<IExtent> Extents = [];

    static ExtentPersistence()
    {
        DiscoverExtents();
    }

    private static void DiscoverExtents()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var potentialTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic);
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;

        foreach (var type in potentialTypes)
        {
            foreach (var field in type.GetFields(flags).Where(p => typeof(IExtent).IsAssignableFrom(p.FieldType)))
            {
                field.GetValue(null);
            }
            
            foreach (var property in type.GetProperties(flags).Where(p => typeof(IExtent).IsAssignableFrom(p.PropertyType)))
            {
                property.GetValue(null);
            }
        }
    }

    public static void Register(IExtent extent)
    {
        Extents.Add(extent);
    }

    private static JsonNode _read()
    {
        if (!File.Exists(Path)) return JsonNode.Parse("{}")!;

        var text = File.ReadAllText(Path);
        var doc = JsonNode.Parse(text);

        return doc ?? throw new Exception("Failed to load stored json.");
    }

    public static void Persist(IExtent extent)
    {
        var node = _read();
        node[extent.Name] = extent.ToJson();
        PersistAll(node);
    }

    private static void PersistAll(JsonNode json)
    {
        if (!File.Exists(Path))
        {
            File.AppendAllText(Path, json.ToJsonString());
        }
        else
        {
            File.WriteAllText(Path, json.ToJsonString());
        }
    }

    public static void LoadAll()
    {
        var data = _read();

        foreach (var extent in Extents)
        {
            var classNode = data[extent.Name];

            if (classNode is JsonArray array)
            {
                // create one by one through validation
                extent.LoadFromJsonArray(array);
            }
        }
    }
}