namespace BytChineseSteam.Models;

public class Game
{
    private static readonly List<Game> _viewAllGames = new List<Game>();
    public static IReadOnlyList<Game> ViewAllGames => _viewAllGames.AsReadOnly();

    public string Title { get; private set; }
    public Publisher Publisher { get; private set; } // required, set in ctor

    public Game(string title, Publisher publisher)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        _viewAllGames.Add(this);
    }

    public override string ToString() => $"Game(Title={Title}, Publisher={Publisher.Name})";
}