namespace BytChineseSteam.Models;

public class Manager : Employee
{
    public static readonly decimal PromotionBonus = 100;
    
    private static readonly List<Manager> _managers = new List<Manager>();

    protected Manager(Name name, string email, string phoneNumber, string hashedPassword) : base(name, email, phoneNumber, hashedPassword) { }
    
    protected Manager(Name name, string email, string phoneNumber, string hashedPassword, decimal salary) : base(name, email, phoneNumber, hashedPassword, salary) { }
    
    public Manager() { }
    
    public static void AddManager(Manager manager)
    {
        _managers.Add(manager);
    }
}