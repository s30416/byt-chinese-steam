using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Admin), "Admin")]
[JsonDerivedType(typeof(Manager), "Manager")]
[JsonDerivedType(typeof(SuperAdmin), "SuperAdmin")]
// todo: implement inheritance later
public abstract class Employee
{
    public Name? Name { get; set; }
    
    public decimal? Salary { get; set; }

    public decimal CollectedBonuses { get; set; } = 0;

    private static List<Employee> _employees = new();
    
    protected Employee(Name name, decimal? salary)
    {
        Name = name;
        Salary = salary;
        
        // add to collection
        AddEmployee(this);
    }

    protected Employee(Name name)
    {
        Name = name;
        
        // add to collection
        AddEmployee(this);
    }
    
    // required for deserialization
    [JsonConstructor]
    protected Employee()
    {
        // entent
        AddEmployee(this);
    }
    
    // extent methods
    public static ReadOnlyCollection<Employee> ViewAllEmployees()
    {
        return _employees.AsReadOnly();
    }

    // never use this outside of constructors. when you call new() the newly created object will be added to the
    // collection right after creation. So now also, be VERY CAREFUL when you use new()
    private static void AddEmployee(Employee employee)
    {
        if (employee == null)
            throw new ArgumentException($"The given employee cannot be null");
        
        _employees.Add(employee);
    }
    
    // class methods
    // will be required to change the isSuperAdmin bool to an actual check on the controller/service layer
    // I will also have to figure out the use of generics here, but that's inheritance issues
    public static Employee CreateEmployee<T>(string firstName, string lastName, decimal? salary, bool isSuperAdmin)
    {
        if (!isSuperAdmin)
            throw new UnauthorizedAccessException("Only super admins can create Employees");

        var name = new Name(firstName, lastName);
        if (typeof(T) == typeof(Admin))
        {
            if (salary == null) return new Admin(name);
            else return new Admin(name, (decimal)salary);
        } 
        else if (typeof(T) == typeof(Manager))
        {
            if (salary == null) return new Manager(name);
            else return new Manager(name, (decimal)salary);
        } 
        else if (typeof(T) == typeof(SuperAdmin))
        {
            if (salary == null) return new SuperAdmin(name);
            else return new SuperAdmin(name, (decimal)salary);
        }
        else
        {
            throw  new ArgumentException($"The given employee type  {typeof(T)} is not supported.");
        }
    }
}