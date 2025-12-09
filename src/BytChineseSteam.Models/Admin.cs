using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models;

public class Admin(Name name, string email, string phoneNumber, string hashedPassword, decimal? salary)
    : Employee(name, email, phoneNumber, hashedPassword, salary)
{
    public static readonly decimal GameBonus = 500;

    private static List<Admin> _admins = new();
    
    private readonly HashSet<Game> _games = new();
    
    [JsonIgnore] 
    public IReadOnlyCollection<Game> Games => _games.ToList().AsReadOnly();

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
    
    // game association
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