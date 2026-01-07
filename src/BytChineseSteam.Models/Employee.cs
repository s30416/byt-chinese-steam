using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using BytChineseSteam.Models.DataAnnotations;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public class Employee : User
{
    private static readonly Extent<Employee> Extent = new();

    [NonNegative] public decimal? Salary { get; set; }
    
    [JsonInclude]
    public SuperAdmin? Creator { get; internal set; }
    
    // Composition
    
    [JsonInclude] public Admin? AdminRole { get; private set; }
    [JsonInclude] public Manager? ManagerRole { get; private set; }
    [JsonInclude] public SuperAdmin? SuperAdminRole { get; private set; }

    public decimal GetCollectedBonuses()
    {
        decimal total = 0;
        if (AdminRole != null) total += Admin.GameBonus;
        if (ManagerRole != null) total += Manager.PromotionBonus;
        return total;
    }

    public Employee(Name name, string email, string phoneNumber, string hashedPassword, decimal? salary, SuperAdmin? creator = null) : base(name, email, phoneNumber, hashedPassword)
    {
        Salary = salary;
        
        // making connection 
        Creator = creator;
        if (Creator != null)
        {
            Creator.AddCreatedEmployee(this);
        }
        
        // add to collection
        Extent.Add(this);
    }
    
    
    // Composition methods for assigning and unassigning role
    
    public void AssignAdminRole(Admin admin)
    {
        if (AdminRole != null) throw new InvalidOperationException("Employee is already an Admin.");
        AdminRole = admin;
    }

    public void UnassignAdminRole() => AdminRole = null;

    public void AssignManagerRole(Manager manager)
    {
        if (ManagerRole != null) throw new InvalidOperationException("Employee is already a Manager.");
        ManagerRole = manager;
    }
    
    public void UnassignManagerRole() => ManagerRole = null;

    public void AssignSuperAdminRole(SuperAdmin superAdmin)
    {
        if (SuperAdminRole != null) throw new InvalidOperationException("Employee is already a SuperAdmin.");
        SuperAdminRole = superAdmin;
    }
    
    public void UnassignSuperAdminRole() => SuperAdminRole = null;

    // extent methods
    public static ReadOnlyCollection<Employee> ViewAllEmployees()
    {
        return Extent.All();
    }

    // never use this outside of constructors. when you call new() the newly created object will be added to the
    // collection right after creation. So now also, be VERY CAREFUL when you use new()
    private static void AddEmployee(Employee employee)
    {
        if (employee == null)
            throw new ArgumentException($"The given employee cannot be null");

        Extent.Add(employee);
    }

    // class methods
    // will be required to change the isSuperAdmin bool to an actual check on the controller/service layer
    // I will also have to figure out the use of generics here, but that's inheritance issues
    public static Employee CreateEmployee<T>(string firstName, string lastName, string email, string phoneNumber,
        string password, decimal? salary, SuperAdmin creator)
    {
        if (creator == null)
            throw new UnauthorizedAccessException("Only super admins can create Employees");

        var name = new Name(firstName, lastName);
        
        // base employee (container)
        var employee = new Employee(name, email, phoneNumber, password, salary, creator);
        
        if (typeof(T) == typeof(Admin))
        {
            new Admin(employee); 
        }
        else if (typeof(T) == typeof(Manager))
        {
            new Manager(employee);
        }
        else if (typeof(T) == typeof(SuperAdmin))
        {
            new SuperAdmin(employee);
        }
        else
        {
            throw new ArgumentException($"The given employee type {typeof(T)} is not supported.");
        }

        return employee;
    }
}