using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models.Util;
using BytChineseSteam.Repository.Extent;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models;

public class Game
{
    private static readonly Extent<Game> Extent = new();

    [MinLength(1)] [Required] public string Title { get; private set; }
    
    public string? Description { get; private set; }
    
    [MinLength(1)] [Required] public string GameSlug { get; private set; } 
    [JsonIgnore] public Admin Admin { get; private set; }
    
    // reverse connections (if you're reading this comment: close this code right now and never come back)
    private readonly HashSet<Category> _categories = new();
    
    public Publisher Publisher { get; private set; }
    
    private readonly HashSet<Key> _keys = new();

    [JsonIgnore]
    public IReadOnlyCollection<Key> Keys => _keys.ToList().AsReadOnly();

    private Game() { }
    
    [JsonConstructor]
    public Game(string title, string? description, Category? category, Publisher publisher, Admin admin)
    {
        Title = title;
        Description = description;
        GameSlug = Slugifier.ToGameSlug(title);
        Publisher = publisher;
        
        if (admin == null) throw new ArgumentNullException(nameof(admin), "Game must have an Admin.");
        Admin = admin;
        Admin.AddGame(this);

        if (category != null)
        {
            category.AddGame(this);
            _categories.Add(category);
        }

        Extent.Add(this);
    }

    public Game(string title, string? description, Publisher publisher, Admin admin)
    {
        Title = title;
        Description = description;
        GameSlug = Slugifier.ToGameSlug(title);
        Publisher = publisher;
        
        if (admin == null) throw new ArgumentNullException(nameof(admin), "Game must have an Admin.");

        Admin = admin;
        Admin.AddGame(this);
        
        Extent.Add(this);
    }
    

    internal void AddKey(Key key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        
        if (key.Game != this)
        {
            throw new InvalidOperationException("Cannot add a key that belongs to another game.");
        }

        if (!_keys.Contains(key))
        {
            _keys.Add(key);
        }
    }

    internal void RemoveKey(Key key)
    {
        _keys.Remove(key);
    }

    public void DeleteGame()
    {
        foreach (var key in _keys.ToList())
        {
            key.DeleteKey(); 
        }
        
        if (Admin != null)
        {
            Admin.RemoveGame(this);
        }
        
        Extent.Remove(this);
    }

    public void AddCategory(Category category)
    {
        if (category == null) throw new ArgumentNullException(nameof(category));
        if (_categories.Contains(category)) return;

        _categories.Add(category);

        // reverse connection
        if (!category.ContainsGame(this))
        {
            category.AddGame(this);
        }
    }

    public void RemoveCategory(Category category)
    {
        if (category == null) throw new ArgumentNullException(nameof(category));
        if (!_categories.Contains(category)) return;

        _categories.Remove(category);

        // reverse connection
        if (category.ContainsGame(this))
        {
            category.RemoveGame(this);
        }
    }

    // methods
    public IReadOnlyList<Category> GetAllCategoriesForGame() => _categories.ToList().AsReadOnly();
    
    public static IReadOnlyList<Game> ViewAllGames => Extent.All();

    public override string ToString() => $"Game(Title={Title}, Publisher={Publisher.Name})";
    
    // admin association
    
    // since game MUST have an admin, we only give option to change it - not remove
    public void ChangeAdmin(Admin newAdmin)
    {
        if (newAdmin == null) throw new ArgumentNullException(nameof(newAdmin), "New Admin cannot be null.");
        
        if (Admin == newAdmin) return; // No change needed
        // remove game from old admin
        Admin.RemoveGame(this);
        // change to new admin
        Admin = newAdmin;
        // add to new admin
        Admin.AddGame(this);
    }
}