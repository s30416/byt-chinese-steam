using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public class Admin : Employee
{
    public static Extent<Admin> Extent = new();
    
    public static readonly decimal GameBonus = 500;

    // publisher association
    [JsonIgnore]
    private ISet<Publisher> _publishers = new HashSet<Publisher>();
    
    [JsonIgnore] 
    public IReadOnlyCollection<Game> Games => _games.ToList().AsReadOnly();
    
    [JsonConstructor]
    public Admin(Name name, string email, string phoneNumber, string hashedPassword, decimal? salary, ISet<Publisher> publishers) : base(name, email, phoneNumber, hashedPassword, salary)
    {
        AddAdmin(this);

        foreach (var publisher in publishers)
        {
            AddPublisher(publisher);
        }
    }

    public Admin(Name name, string email, string phoneNumber, string hashedPassword, decimal? salary) : this(name, email, phoneNumber, hashedPassword, salary, new HashSet<Publisher>())
    {
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
    
    // publisher association
    internal void AddPublisher(Publisher publisher)
    {
        ArgumentNullException.ThrowIfNull(publisher);

        if (_publishers.Contains(publisher))
        {
            throw new ArgumentException($"The given publisher already exists");
        }
        
        _publishers.Add(publisher);
    }
    
    internal void RemovePublisher(Publisher publisher)
    {
        ArgumentNullException.ThrowIfNull(publisher);

        if (!_publishers.Contains(publisher))
        {
            throw new ArgumentException($"This admin did not create given publisher");
        }
        
        _publishers.Remove(publisher);
    }
}