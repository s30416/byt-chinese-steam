using System.Collections.ObjectModel;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public class Category
{
    public static readonly Extent<Category> Extent = new ();

    // the qualifier (key) is a game slug
    private readonly Dictionary<string, Game> _games = new();
    
    public string Name { get; private set; }

    public Category(string name)
    {
        Name = name;
    }
    
    // dictionary methods
    
    public void AddGame(Game game)
    {
        if (!_games.ContainsKey(game.GameSlug))
        {
            _games.Add(game.GameSlug, game);
            game.AddCategory(this);
        }
    }

    public void AddGames(IEnumerable<Game> games)
    {
        foreach (var game in games)
        {
            AddGame(game);
        }
    }

    // removing games and removing their categories
    public void RemoveGame(Game game)
    {
        if (_games.Remove(game.GameSlug))
        {
            game.RemoveCategory(this);
        }
    }
    
    public void RemoveGame(string gameSlug)
    {
        if (_games.TryGetValue(gameSlug, out var game))
        {
            _games.Remove(gameSlug);
            game.RemoveCategory(this);
        }
    }
    
    // class methods
    
    // get the list of games in category
    public ReadOnlyCollection<Game> GetAllGamesInCategory => _games.Values.ToList().AsReadOnly();
    
    
}