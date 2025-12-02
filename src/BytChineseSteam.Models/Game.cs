using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models.Util;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public class Game
{
    private static readonly Extent<Game> Extent = new();
    
    public static IReadOnlyList<Game> ViewAllGames => Extent.All();

    [MinLength(1)] [Required] public string Title { get; private set; }
    
    public string? Description { get; private set; }
    
    [MinLength(1)] [Required] public string GameSlug { get; private set; } // created using Slugifier class in Utils
    
    // reverse connections
    public Category? Category { get; set; } // nullabe because aggregation (should the setter be private??)
    
    public Publisher Publisher { get; private set; }

    public Game(string title, string? description, Category? category, Publisher publisher)
    {
        Title = title;
        Description = description;
        GameSlug = Slugifier.ToGameSlug(title);
        Category = category;
        Publisher = publisher;
        
        if (category != null) category.AddGame(this);

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

    // methods

    public override string ToString() => $"Game(Title={Title}, Publisher={Publisher.Name})";
}