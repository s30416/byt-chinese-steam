using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models;

public class SuperAdmin : Employee
{
    private static List<SuperAdmin> _superAdmins = new();
    
    [JsonIgnore]
    private readonly HashSet<Employee> _createdEmployees = new();
    
    [JsonIgnore]
    public IReadOnlyCollection<Employee> CreatedEmployees => _createdEmployees.ToList().AsReadOnly();

    // extent methods
    
    [JsonConstructor]
    public SuperAdmin(User user, decimal? salary, SuperAdmin? creator = null)
        : base(user, salary, creator)
    {
        AddSuperAdmin(this);
    }

    public static ReadOnlyCollection<SuperAdmin> ViewAllSuperAdmins()
    {
        return _superAdmins.AsReadOnly();
    }
    
    private static void AddSuperAdmin(SuperAdmin superAdmin)
    {
        if (superAdmin == null)
            throw new ArgumentException($"The given employee cannot be null");
        
        _superAdmins.Add(superAdmin);
    }
    
    // new methods for SuperAdmin-Employee association

    public void AddCreatedEmployee(Employee employee)
    {
        if (employee == null) throw new ArgumentNullException(nameof(employee));

        if (employee.Creator != null && employee.Creator != this)
        {
            throw new InvalidOperationException("Employee creator mismatch.");
        }

        if (_createdEmployees.Contains(employee)) return;

        _createdEmployees.Add(employee);

        if (employee.Creator == null)
        {
            employee.Creator = this;
        }
    }

    internal void RemoveCreatedEmployee(Employee employee)
    {
        if (employee == null) throw new ArgumentNullException(nameof(employee));
        if (!_createdEmployees.Contains(employee)) return;

        _createdEmployees.Remove(employee);

        if (employee.Creator == this)
        {
            employee.Creator = null;
        }
    }
    
    // class methods
}