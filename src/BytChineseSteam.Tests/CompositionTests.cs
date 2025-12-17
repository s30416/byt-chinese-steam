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
            var admin = new Admin(new Name("Big", "Tommy"), "big.tommy@example.com", "+48123456789", "howdoesourhashedpasswork", 5000);
            var publisher = new Publisher("Ubisoft", "Global Publisher", admin);
            var game = new Game("Far Cry 6", "Shooter", publisher, admin);

            var key = new RegularKey(game, admin, "123-ABC", 59.99m, DateTime.Now, 0);
            
            Assert.That(key.Game, Is.EqualTo(game));
            Assert.That(game.Keys, Does.Contain(key));
            Assert.That(game.Keys.Count, Is.EqualTo(1));
        }

        [Test]
        public void CreateKey_WithNullGame_ShouldThrowException()
        {
            var admin = new Admin(new Name("Big", "Tommy"), "big.tommy@example.com", "+48123456789", "howdoesourhashedpasswork", 5000);
            
            var ex = Assert.Throws<ArgumentNullException>(() => 
                new RegularKey(null!, admin, "123-ABC", 59.99m, DateTime.Now, 0));
            
            Assert.That(ex!.ParamName, Is.EqualTo("game"));
        }

        [Test]
        public void DeleteGame_ShouldCascadeDeleteKeys()
        {
            var admin = new Admin(new Name("Big", "Tommy"), "big.tommy@example.com", "+48123456789", "howdoesourhashedpasswork", 5000);
            var publisher = new Publisher("EA", "Sports Publisher", admin);
            var game = new Game("FIFA 24", "Sports", publisher, admin);
            var key1 = new RegularKey(game, admin, "KEY-1", 10, DateTime.Now, 0);
            var key2 = new RegularKey(game, admin, "KEY-2", 20, DateTime.Now, 0);
            
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
            var admin = new Admin(new Name("Big", "Tommy"), "big.tommy@example.com", "+48123456789", "howdoesourhashedpasswork", 5000);
            var publisher = new Publisher("Valve", "PC Platform", admin);
            var game = new Game("Portal 2", "Puzzle", publisher, admin);
            var key = new RegularKey(game, admin, "P2-KEY", 10, DateTime.Now, 0);
            
            key.DeleteKey();

            Assert.That(Key.Extent.All(), Does.Not.Contain(key));
            Assert.That(game.Keys, Does.Not.Contain(key));
            Assert.That(game.Keys, Is.Empty);
            Assert.That(Game.ViewAllGames, Does.Contain(game));
        }
    }
}