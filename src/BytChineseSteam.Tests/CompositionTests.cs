using BytChineseSteam.Models;

namespace BytChineseSteam.Tests
{
    public class CompositionTests
    {
        [SetUp]
        public void Setup()
        {
            var allGames = Game.ViewAllGames.ToList();
            foreach (var game in allGames)
            {
                game.DeleteGame(); 
            }
        }

        [Test]
        public void CreateKey_ShouldAutomaticallyAddToGame()
        {
            var publisher = new Publisher("Ubisoft", "Global Publisher");
            var game = new Game("Far Cry 6", "Shooter", publisher);

            var key = new Key(game, "123-ABC", 59.99m, DateTime.Now, 0, new List<string> { "Base Game" });

            Assert.That(key.Game, Is.EqualTo(game));
            Assert.That(game.Keys, Does.Contain(key));
            Assert.That(game.Keys.Count, Is.EqualTo(1));
        }

        [Test]
        public void CreateKey_WithNullGame_ShouldThrowException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => 
                new Key(null!, "123-ABC", 59.99m, DateTime.Now, 0, new List<string> { "Base Game" }));
            
            Assert.That(ex!.Message, Does.Contain("Key cannot exist without a Game"));
        }

        [Test]
        public void DeleteGame_ShouldCascadeDeleteKeys()
        {
            var publisher = new Publisher("EA", "Sports Publisher");
            var game = new Game("FIFA 24", "Sports", publisher);
            var key1 = new Key(game, "KEY-1", 10, DateTime.Now, 0, new List<string>{"A"});
            var key2 = new Key(game, "KEY-2", 20, DateTime.Now, 0, new List<string>{"B"});

            Assert.That(Key.Extent.All().Count, Is.EqualTo(2));
            Assert.That(Game.ViewAllGames.Count, Is.EqualTo(1));

            game.DeleteGame();

            Assert.That(Game.ViewAllGames, Does.Not.Contain(game));
            Assert.That(Key.Extent.All(), Does.Not.Contain(key1));
            Assert.That(Key.Extent.All(), Does.Not.Contain(key2));
            Assert.That(Key.Extent.All(), Is.Empty);
        }

        [Test]
        public void DeleteKey_ShouldRemoveFromGame()
        {
            var publisher = new Publisher("Valve", "PC Platform");
            var game = new Game("Portal 2", "Puzzle", publisher);
            var key = new Key(game, "P2-KEY", 10, DateTime.Now, 0, new List<string>{"A"});

            key.DeleteKey();

            Assert.That(Key.Extent.All(), Does.Not.Contain(key));
            Assert.That(game.Keys, Does.Not.Contain(key));
            Assert.That(game.Keys, Is.Empty);
            Assert.That(Game.ViewAllGames, Does.Contain(game));
        }
    }
}