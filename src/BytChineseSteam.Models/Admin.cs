using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public class Admin : Employee
{
    public static Extent<Admin> Extent = new();
    
    public static readonly decimal GameBonus = 500;

    
    [JsonIgnore] 
    public IReadOnlyCollection<Game> Games => _games.ToList().AsReadOnly();
    
    public Admin(Name name, string email, string phoneNumber, string hashedPassword, decimal? salary) : base(name, email, phoneNumber, hashedPassword, salary)
    {
        AddAdmin(this);
    }

    // extent methods

    public static ReadOnlyCollection<Admin> ViewAllAdmins()
    {
        return Extent.All();
    }

    private static void AddAdmin(Admin admin)
    {
        if (admin == null)
            throw new ArgumentException($"The given employee cannot be null");

        Extent.Add(admin);
    }

    // class methods
    
    // game association
    private readonly HashSet<Game> _games = new();
    
    internal void AddGame(Game game)
    {
        if (game == null) throw new ArgumentNullException(nameof(game));
        
        if (!_games.Contains(game))
        {
            _games.Add(game);
        }
    }

    internal void RemoveGame(Game game)
    {
        if (game == null) throw new ArgumentNullException(nameof(game));
        
        _games.Remove(game);
    }
}