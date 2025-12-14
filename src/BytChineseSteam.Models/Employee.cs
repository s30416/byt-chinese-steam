using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using BytChineseSteam.Models.DataAnnotations;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public class Employee
{
    private static readonly Extent<Employee> Extent = new();

    [NonNegative] public decimal? Salary { get; set; }
    
    [JsonInclude]
    public SuperAdmin? Creator { get; internal set; }
    
    // inheritance
    [JsonIgnore]
    public User User { get; private set; }

    public decimal GetCollectedBonuses()
    {
        return 0;
    }

    public Employee(User user, decimal? salary, SuperAdmin? creator = null)
    {
        Salary = salary;
        
        // making connection 
        Creator = creator;
        if (Creator != null)
        {
            Creator.AddCreatedEmployee(this);
        }
        
        // inheritance
        User = user ?? throw new ArgumentNullException(nameof(user));
        user.AddEmployee(this); // reverse connection
        Extent.Add(this);
        
        // add to collection
        Extent.Add(this);
    }

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
        if (typeof(T) == typeof(Admin))
        {
            if (salary == null) return new Admin(new User(name, email, phoneNumber, password), null);
            else return new Admin(new User(name, email, phoneNumber, password), (decimal)salary, creator);
        }
        else if (typeof(T) == typeof(Manager))
        {
            if (salary == null) return new Manager(new User(name, email, phoneNumber, password), null);
            else return new Manager(new User(name, email, phoneNumber, password), (decimal)salary, creator);
        }
        else if (typeof(T) == typeof(SuperAdmin))
        {
            if (salary == null) return new SuperAdmin(new User(name, email, phoneNumber, password), null);
            else return new SuperAdmin(new User(name, email, phoneNumber, password), (decimal)salary, creator);
        }
        else
        {
            throw new ArgumentException($"The given employee type  {typeof(T)} is not supported.");
        }
    }
    
    public void ChangeUser(User newUser)
    {
        throw new InvalidOperationException("Customer cannot change its User, since it's inheritance.");
    }

    public static void RemoveEmployee(Employee employee)
    {
        if (employee == null)
            throw new ArgumentException($"The given employee cannot be null");
        
        Extent.Remove(employee);
        employee.User.RemoveEmployee(employee);
    }
}