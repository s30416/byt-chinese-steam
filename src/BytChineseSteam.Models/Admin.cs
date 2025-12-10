using System.Collections.Immutable;
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
    public ImmutableHashSet<Publisher> Publishers => _publishers.ToImmutableHashSet();
    [JsonIgnore]
    private readonly ISet<Publisher> _publishers = new HashSet<Publisher>();

    [JsonIgnore] 
    public IReadOnlyCollection<Game> Games => _games.ToList().AsReadOnly();
    
    
    // for Admin-Key association
    [JsonIgnore]
    private readonly HashSet<Key> _createdKeys = new();

    [JsonIgnore]
    public IReadOnlyCollection<Key> CreatedKeys => _createdKeys.ToList().AsReadOnly();

    
    [JsonConstructor]
    public Admin(Name name, string email, string phoneNumber, string hashedPassword, decimal? salary, SuperAdmin? creator = null) 
        : base(name, email, phoneNumber, hashedPassword, salary, creator)
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

    public Admin(Name name, string email, string phoneNumber, string hashedPassword, decimal? salary) : base()
    {
    }

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
        
        if (!_publishers.Add(publisher))
        {
            throw new ArgumentException($"The given publisher already exists");
        }
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

    public void AddCreatedKey(Key key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (key.Creator != this)
        {
            throw new InvalidOperationException("Key creator mismatch. Cannot add key created by another admin.");
        }

        if (!_createdKeys.Contains(key))
        {
            _createdKeys.Add(key);
        }
    }

    internal void RemoveCreatedKey(Key key)
    {
        ArgumentNullException.ThrowIfNull(key);
        _createdKeys.Remove(key);
    }
}