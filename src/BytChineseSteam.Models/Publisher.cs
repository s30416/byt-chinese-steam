/* TODO:
 * possibly annotation that makes function only for admins
*/
namespace BytChineseSteam.Models;

public class Publisher
{
    public string Name { get; set; }
    public string Description { get; set; }

    private static readonly List<Publisher> _publishers = new List<Publisher>();

    private Publisher(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public Publisher() { }

    public static IReadOnlyList<Publisher> ViewAllPublishers()
    {
        return _publishers.AsReadOnly();
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

        var publisher = new Publisher(name.Trim(), description ?? "");
        _publishers.Add(publisher);
        return publisher;
    }

    public static void DeletePublisher(string name, bool isAdmin)
    {
        if (!isAdmin)
            throw new UnauthorizedAccessException("Only admin can delete publishers.");

        var publisher = _publishers.FirstOrDefault(p => p.Name == name);
        if (publisher == null)
            throw new InvalidOperationException("Publisher not found.");

        _publishers.Remove(publisher);
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
}