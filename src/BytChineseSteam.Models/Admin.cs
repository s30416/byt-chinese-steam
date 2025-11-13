namespace BytChineseSteam.Models;

public class Admin : Employee
{
    private static readonly decimal GameBonus = 500;
    
    private static readonly List<Admin> _admins = new List<Admin>();
    
    protected Admin(Name name, string email, string phoneNumber, string hashedPassword) : base(name, email, phoneNumber, hashedPassword) { }
    
    protected Admin(Name name, string email, string phoneNumber, string hashedPassword, decimal salary) : base(name, email, phoneNumber, hashedPassword, salary) { }
    
    public static void AddAdmin(Admin admin)
    {
        _admins.Add(admin);
    }
}