namespace BytChineseSteam.Models;

public class SuperAdmin : Employee
{
    private static readonly List<SuperAdmin> _superAdmins = new List<SuperAdmin>();
    
    protected SuperAdmin(Name name, string email, string phoneNumber, string hashedPassword) : base(name, email, phoneNumber, hashedPassword) { }
    
    protected SuperAdmin(Name name, string email, string phoneNumber, string hashedPassword, decimal salary) : base(name, email, phoneNumber, hashedPassword, salary) { }
    
    public SuperAdmin() { }

    public static void AddSuperAdmin(SuperAdmin superAdmin)
    {
        _superAdmins.Add(superAdmin);
    }
}