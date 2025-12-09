using System.Collections.ObjectModel;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public class Category
{
    public static readonly Extent<Category> Extent = new ();

    // the qualifier (key) is a game slug
    private readonly Dictionary<string, Game> _games = new();
    internal bool ContainsGame(Game game) => _games.ContainsKey(game.GameSlug);
    
    public string Name { get; private set; }

    public Category(string name)
    {
        Name = name;
    }
    
    public void AddGame(Game game)
    {
        if (game == null) throw new ArgumentNullException(nameof(game));
        if (_games.ContainsKey(game.GameSlug)) return;

        _games.Add(game.GameSlug, game);

        // reverse connection
        if (!game.GetAllCategoriesForGame().Contains(this))
        {
            game.AddCategory(this);
        }
    }

    public void RemoveGame(Game game)
    {
        if (game == null) throw new ArgumentNullException(nameof(game));
        if (!_games.Remove(game.GameSlug)) return;

        // reverse connection
        if (game.GetAllCategoriesForGame().Contains(this))
        {
            game.RemoveCategory(this);
        }
    }
    
    // class methods
    
    // get the list of games in category
    public ReadOnlyCollection<Game> GetAllGamesInCategory => _games.Values.ToList().AsReadOnly();
    
    
}