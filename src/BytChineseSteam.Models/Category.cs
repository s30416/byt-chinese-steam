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
    
    // adds the game to the dictionary if it is not already there and resets the game's category
    // if the game is already in the category, then we ensure it has the correct category set
    public void AddGame(Game game)
    {
        if (!_games.ContainsKey(game.GameSlug))
        {
            _games.Add(game.GameSlug, game);
            game.Category = this;   
        }
        else
        {
            game.Category = this;
        }
    }

    // adds all the games from the list that are not already added and resets its category
    // if the game is already in the category, then we ensure it has the correct category set
    public void AddGames(IEnumerable<Game> games)
    {
        foreach (var game in games)
        {
            if (!_games.ContainsKey(game.GameSlug))
            {
                _games.Add(game.GameSlug, game);
                game.Category = this;   
            }
            else
            {
                game.Category = this;
            }
        }
    }

    // removing games and removing their categories
    public void RemoveGame(Game game)
    {
        var removedGame = _games[game.GameSlug];
        _games.Remove(game.GameSlug);
        removedGame.Category = null;
    }
    
    public void RemoveGame(string gameSlug)
    {
        var removedGame = _games[gameSlug];
        _games.Remove(gameSlug);
        removedGame.Category = null;
    }
    
    
    // class methods
    
    // get the list of games in category
    public ReadOnlyCollection<Game> GetALlGamesInCategory => _games.Values.ToList().AsReadOnly();
    
    
}