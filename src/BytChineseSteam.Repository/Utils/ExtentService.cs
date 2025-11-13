using System.Text.Json;
using System.Text.Json.Serialization;
using BytChineseSteam.Models;

namespace BytChineseSteam.Repository.Utils
{
    public static class ExtentService
    {
        // placeholder for now — later replace with config value from appsettings.json
        private static readonly string BasePath = "C:\\Users\\Andrii Sysoiev\\RiderProjects\\byt-chinese-steam\\src\\BytChineseSteam.Repository\\Storage";

        private static readonly string EmployeesPath = Path.Combine(BasePath, "employees.json");
        private static readonly string LimitedKeysPath = Path.Combine(BasePath, "limited_keys.json");
        private static readonly string PublishersPath = Path.Combine(BasePath, "publishers.json");
        private static readonly string UsersPath = Path.Combine(BasePath, "users.json");

        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        };

        public static async Task<bool> SaveObjectAsync<T>(object obj)
        {
            try
            {
                var path = GetPathForType(typeof(T));
                var json = JsonSerializer.Serialize(obj, Options);

                await File.WriteAllTextAsync(path, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving {typeof(T).Name}: {ex.Message}");
                return false;
            }
        }

        public static async Task<T?> LoadObjectAsync<T>()
        {
            try
            {
                var path = GetPathForType(typeof(T));
                if (!File.Exists(path)) return default;

                var json = await File.ReadAllTextAsync(path);
                return JsonSerializer.Deserialize<T>(json, Options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {typeof(T).Name}: {ex.Message}");
                return default;
            }
        }

        private static string GetPathForType(Type type)
        {
            if (type == typeof(List<Employee>))
                return EmployeesPath;
            if (type == typeof(List<Publisher>))
                return PublishersPath;
            if (type == typeof(List<Limited>))
                return LimitedKeysPath;
            if (type == typeof(List<User>))
                return UsersPath;

            throw new ArgumentException($"No path mapping defined for type {type.Name}");
        }
    }
}
