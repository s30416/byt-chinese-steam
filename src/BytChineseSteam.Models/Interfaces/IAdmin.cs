namespace BytChineseSteam.Models.Interfaces;

public interface IAdmin
{
    void AddGame(Game game);
    void RemoveGame(Game game);
    void AddPublisher(Publisher publisher);
    void RemovePublisher(Publisher publisher);
    void AddCreatedKey(Key key);
    void RemoveCreatedKey(Key key);
}