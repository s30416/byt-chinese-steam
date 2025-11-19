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

    public static void DiscoverExtents()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;

        foreach (var assembly in assemblies)
        {
            try
            {
                var potentialTypes = assembly.GetTypes();

                foreach (var type in potentialTypes)
                {
                    if (!type.IsClass || type.IsAbstract) continue;

                    foreach (var field in type.GetFields(flags))
                    {
                        if (typeof(IExtent).IsAssignableFrom(field.FieldType))
                        {
                            field.GetValue(null); 
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Handle cases where an assembly might not load all types due to missing dependencies.
                Console.WriteLine($"Warning: Failed to load all types from {assembly.FullName}");
                // Process the loaded types: ex.Types.Where(t => t != null) 
            }
            catch (Exception)
            {
                // Ignore other assembly loading errors
            }
        }
    }

    public static void Register(IExtent extent)
    {
        if (Extents.Any(e => e.Name == extent.Name))
        {
            throw new ArgumentException($"Extent with name {extent.Name} already exists");
        }
        
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