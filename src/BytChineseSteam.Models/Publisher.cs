/* TODO:
 * possibly annotation that makes function only for admins
*/

using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models;

public class Publisher
{
    private static readonly Extent<Publisher> Extent = new();
    public string Name { get; set; }
    public string Description { get; set; }
    
    public Publisher(string name, string description)
    {
        Name = name;
        Description = description;
        
        Extent.Add(this);
    }
    
    public static Publisher CreatePublisher(string name, string description, bool isAdmin)
    {
        if (!isAdmin)
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

        var publisher = new Publisher(name.Trim(), description ?? "");
        return publisher;
    }

    public static void DeletePublisher(string name, bool isAdmin)
    {
        if (!isAdmin)
            throw new UnauthorizedAccessException("Only admin can delete publishers.");

        var publisher = Extent.All().FirstOrDefault(p => p.Name == name);
        
        if (publisher == null)
            throw new InvalidOperationException("Publisher not found.");

        Extent.Remove(publisher);
    }

    public void UpdatePublisher(string newName, string newDescription, bool isAdmin)
    {
        if (!isAdmin)
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