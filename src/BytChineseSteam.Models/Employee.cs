using BytChineseSteam.Models.DataAnnotations;

namespace BytChineseSteam.Models;

public abstract class Employee : User
{
    [NonNegative]
    public decimal? Salary { get; set; }

    [NonNegative]
    public decimal CollectedBonuses { get; set; } = 0;

    private static readonly List<Employee> _employees = new List<Employee>();
    
    protected Employee(Name name, string email, string phoneNumber, string hashedPassword, decimal? salary) : base(name, email, phoneNumber, hashedPassword)
    {
        Salary = salary;
    }
    
    protected Employee(Name name, string email, string phoneNumber, string hashedPassword) : base(name, email, phoneNumber, hashedPassword) { }

    public Employee() {}

    // will be required to change the isSuperAdmin bool to an actual check on the controller/service layer
    public static T CreateEmployee<T>(string firstName, string lastName, string email, string phoneNumber,
        string hashedPassword, decimal? salary, bool isSuperAdmin) where T : Employee, new()
    {
        if (!isSuperAdmin)
            throw new UnauthorizedAccessException("Only super admins can create Employees");

        var name = new Name(firstName, lastName);
        var newEmployee = new T
        {
            Name = name,
            Email = email,
            PhoneNumber = phoneNumber,
            HashedPassword = hashedPassword,
            Salary = salary
        };
        
        _employees.Add(newEmployee);
        if (typeof(T) == typeof(SuperAdmin))
        {
            SuperAdmin.AddSuperAdmin(newEmployee as SuperAdmin);
        } 
        else if (typeof(T) == typeof(Manager))
        {
            Manager.AddManager(newEmployee as Manager);
        }
        else if (typeof(T) == typeof(Admin))
        {
            Admin.AddAdmin(newEmployee as Admin);
        }

        return newEmployee;
    }

    
    // I actually have no idea on how the update should work correctly
    // like, should it take stuff like an email and look for a specific employee
    // or should i just change the one employee I called this method with
    // so many questions, so little sleep...
    // im gonna leave it as it is right now and ask Abdulla today
    public bool UpdateEmployee(string email, decimal salary, decimal collectedBonuses, bool isSuperAdmin)
    {
        if (!isSuperAdmin)
            throw new UnauthorizedAccessException("Only super admins can create Employees");
        
        CollectedBonuses = collectedBonuses;
        Salary = salary;
        return true;
    }
    
    public bool UpdateEmployee(decimal collectedBonuses, bool isSuperAdmin)
    {
        if (!isSuperAdmin)
            throw new UnauthorizedAccessException("Only super admins can create Employees");
        
        CollectedBonuses = collectedBonuses;
        return true;
    }

    public bool DeleteEmployee(bool isSuperAdmin)
    {
        if (!isSuperAdmin)
            throw new UnauthorizedAccessException("Only super admins can create Employees");
        
        _employees.Remove(this);
        return true;
    }
}