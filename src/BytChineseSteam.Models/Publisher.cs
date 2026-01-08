/* TODO:
 * possibly annotation that makes function only for admins
*/

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using BytChineseSteam.Models.Interfaces;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public class Publisher
{
    private static readonly Extent<Publisher> Extent = new();
    [MinLength(1)] [Required] public string Name { get; set; }
    public string Description { get; set; }
    
    // admin association
    [JsonIgnore]
    public IAdmin Admin { get; private set; }
    
    // game association
    private HashSet<Game> _publishedGames = new();
    
    public Publisher(string name, string description, IAdmin admin)
    {
        Name = name;
        Description = description;
        Admin = admin;
        
        Admin.AddPublisher(this);
        
        Extent.Add(this);
    }

    public void AddGame(Game game)
    {
        if (game == null) throw new ArgumentNullException(nameof(game));

        if (_publishedGames.Contains(game)) return;

        _publishedGames.Add(game);

        if (game.Publisher != this)
        {
            game.ChangePublisher(this);
        }
    }
    
    // THESE ARE NOT CHATGPT'S COMMENTS, I WROTE THEM, DO NOT REMOVE THEM
    // Game must always have a Publisher
    // be careful when using this method on its own
    // prefferably use Game.ChangePublisher instead
    public void RemoveGame(Game game)
    {
        if (game == null) throw new ArgumentNullException(nameof(game));

        if (!_publishedGames.Contains(game)) return;
        
        if (game.Publisher == this)
        {
            throw new InvalidOperationException(
                "Cannot remove publisher from a game without assigning a new one. " +
                "Change the publisher first or use Game.ChangePublisher() instead.");
        }

        _publishedGames.Remove(game);
    }
    
    public static Publisher CreatePublisher(string name, string description, Employee actor)
    {
        // because of the fact that now we have roles and no inheritance from employee, we are checking for role, before was "actor is Admin" 
        if (actor.AdminRole == null)
        {
            throw new UnauthorizedAccessException("Only admin can create publisher");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Say. Its. Name... You're god damn right.");
        }
        
        // Check for duplicates
        if (Extent.All().Any(p => p.Name == name.Trim()))
        {
            throw new InvalidOperationException("Publisher with this name already exists.");
        }

        var publisher = new Publisher(name.Trim(), description ?? "", actor.AdminRole);
        return publisher;
    }

    public static void DeletePublisher(string name, Employee actor)
    {
        if (actor.AdminRole == null)
            throw new UnauthorizedAccessException("Only admin can delete publishers.");

        var publisher = Extent.All().FirstOrDefault(p => p.Name == name);
        
        if (publisher == null)
            throw new InvalidOperationException("Publisher not found.");

        Extent.Remove(publisher);
        publisher.Admin.RemovePublisher(publisher);
        Console.WriteLine(publisher.Admin.GetHashCode());
    }

    public void UpdatePublisher(string newName, string newDescription, Employee actor)
    {
        if (actor.AdminRole == null)
            throw new UnauthorizedAccessException("Only admin can update publishers.");

        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Publisher name cannot be empty.");

        Name = newName.Trim();
        Description = newDescription ?? "";
    }

    public IReadOnlyList<Game> GetAllPublishersGames()
    {
        var list = new List<Game>();
        foreach (var g in Game.ViewAllGames)
        {
            if (ReferenceEquals(g.Publisher, this))
                list.Add(g);
        }
        return list.AsReadOnly();
    }
    
    public static ReadOnlyCollection<Publisher> GetAll()
    {
        return Extent.All();
    }
}