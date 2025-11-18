using System.Text.Json.Serialization;
using BytChineseSteam.Models;

namespace BytChineseSteam.Repository.Utils;

public class StorageClass
{
    public List<Employee> Employees { get; set; } = new();
    public List<Admin> Admins { get; set; } = new();
    public List<Manager> Managers { get; set; } = new();
    public List<SuperAdmin> SuperAdmins { get; set; } = new();
    public List<Key> Keys { get; set; } = new();
    public List<User> Users { get; set; } = new();
    public List<Publisher> Publishers { get; set; } = new();
    
    [JsonConstructor]
    public StorageClass() {}
}