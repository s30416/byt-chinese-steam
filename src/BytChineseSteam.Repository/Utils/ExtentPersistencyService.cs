using System.Text.Json;
using System.Text.Json.Serialization;
using BytChineseSteam.Models;

namespace BytChineseSteam.Repository.Utils
{
    public static class ExtentPersistencyService
    {
        // placeholder for now — later replace with config value from appsettings.json
        private static readonly string StoragePath = "C:\\Users\\Andrii Sysoiev\\RiderProjects\\byt-chinese-steam\\src\\BytChineseSteam.Repository\\Storage\\storage.json";

        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        };

        public static async Task SaveFilesAsync()
        {
            try
            {
                // list all entities (there is a reason why i did it like this, i swear it's the best way)
                // add more later
                var storage = new StorageClass
                {
                    Employees = Employee.ViewAllEmployees().ToList(),
                    Admins = Admin.ViewAllAdmins().ToList(),
                    Managers = Manager.ViewAllManagers().ToList(),
                    SuperAdmins = SuperAdmin.ViewAllSuperAdmins().ToList(),
                    Publishers = Publisher.ViewAllPublishers().ToList(),
                };
                
                var json = JsonSerializer.Serialize(storage, Options);

                await File.WriteAllTextAsync(StoragePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving the files: {ex.Message}");
            }
        }

        public static async Task LoadObjectsAsync()
        {
            try
            {
                if (!File.Exists(StoragePath)) 
                    throw new FileNotFoundException("Storage file not found");

                var json = await File.ReadAllTextAsync(StoragePath);
                
                // just deserialize, no need to do anything else
                JsonSerializer.Deserialize<StorageClass>(json, Options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading the files: {ex.Message}");
            }
        }
    }
}
