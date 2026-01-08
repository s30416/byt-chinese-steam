using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace BytChineseSteam.Models.Interfaces;

public interface IAdmin
{
    
    IReadOnlyCollection<Game> Games { get; }
    ImmutableHashSet<Publisher> Publishers { get; }
    
    [JsonIgnore]
    IReadOnlyCollection<Key> CreatedKeys { get; }
    void AddGame(Game game);
    void RemoveGame(Game game);
    void AddPublisher(Publisher publisher);
    void RemovePublisher(Publisher publisher);
    void AddCreatedKey(Key key);
    void RemoveCreatedKey(Key key);
}