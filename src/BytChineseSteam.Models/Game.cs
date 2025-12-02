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
    
    // reverse connections (if you're reading this comment: close this code right now and never come back)
    private readonly HashSet<Category> _categories = new();
    
    public Publisher Publisher { get; private set; }

    
    private readonly HashSet<Key> _keys = new();

    [JsonIgnore]
    public IReadOnlyCollection<Key> Keys => _keys.ToList().AsReadOnly();

    private Game() { }
    
    public Game(string title, string? description, Category? category, Publisher publisher)
    {
        Title = title;
        Description = description;
        GameSlug = Slugifier.ToGameSlug(title);
        Publisher = publisher;

        if (category != null)
        {
            category.AddGame(this);
            _categories.Add(category);
        }

        Extent.Add(this);
    }

    public Game(string title, string? description, Publisher publisher)
    {
        Title = title;
        Description = description;
        GameSlug = Slugifier.ToGameSlug(title);
        Publisher = publisher;
        
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
        
        Extent.Remove(this);
    }

    // the game can be removed from the category only using the category method
    // subject for later change (if you need me to change this - please say so, it will take 15mins)
    internal void AddCategory(Category category)
    {
        _categories.Add(category);
    }

    internal void RemoveCategory(Category category)
    {
        _categories.Remove(category);
    }

    // methods
    public IReadOnlyList<Category> GetAllCategoriesForGame() => _categories.ToList().AsReadOnly();
    
    public static IReadOnlyList<Game> ViewAllGames => Extent.All();

    public override string ToString() => $"Game(Title={Title}, Publisher={Publisher.Name})";
}