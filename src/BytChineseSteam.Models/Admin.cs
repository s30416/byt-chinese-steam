using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models;

public class Admin(Name name, string email, string phoneNumber, string hashedPassword, decimal? salary)
    : Employee(name, email, phoneNumber, hashedPassword, salary)
{
    public static readonly decimal GameBonus = 500;

    private static List<Admin> _admins = new();

    // extent methods

    public static ReadOnlyCollection<Admin> ViewAllAdmins()
    {
        return _admins.AsReadOnly();
    }

    private static void AddAdmin(Admin admin)
    {
        if (admin == null)
            throw new ArgumentException($"The given employee cannot be null");

        _admins.Add(admin);
    }

    // class methods
}