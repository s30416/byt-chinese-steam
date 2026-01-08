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
            var adminEmp = new Employee(
                new Name("Big", "Tommy"), 
                "big.tommy@example.com", 
                "+48123456789", 
                "howdoesourhashedpasswork", 
                5000,
                null
            );
            
            var adminRole = adminEmp.AssignAdminRole();

            var publisher = new Publisher("Ubisoft", "Global Publisher", adminRole);
            var game = new Game("Far Cry 6", "Shooter", publisher, adminRole);

            var key = new RegularKey(game, adminRole, "123-ABC", 59.99m, DateTime.Now, 0);
            
            Assert.That(key.Game, Is.EqualTo(game));
            Assert.That(game.Keys, Does.Contain(key));
            Assert.That(game.Keys.Count, Is.EqualTo(1));
        }

        [Test]
        public void CreateKey_WithNullGame_ShouldThrowException()
        {
            var adminEmp = new Employee(
                new Name("Big", "Tommy"), 
                "big.tommy2@example.com", 
                "+48123456789", 
                "pass", 
                5000,
                null
            );
            var adminRole = adminEmp.AssignAdminRole();
            
            var ex = Assert.Throws<ArgumentNullException>(() => 
                new RegularKey(null!, adminRole, "123-ABC", 59.99m, DateTime.Now, 0));
            
            Assert.That(ex!.ParamName, Is.EqualTo("game"));
        }

        [Test]
        public void DeleteGame_ShouldCascadeDeleteKeys()
        {
            var adminEmp = new Employee(
                new Name("Big", "Tommy"), 
                "big.tommy3@example.com", 
                "+48123456789", 
                "pass", 
                5000,
                null
            );
            var adminRole = adminEmp.AssignAdminRole();

            var publisher = new Publisher("EA", "Sports Publisher", adminRole);
            var game = new Game("FIFA 24", "Sports", publisher, adminRole);
            
            var key1 = new RegularKey(game, adminRole, "KEY-1", 10, DateTime.Now, 0);
            var key2 = new RegularKey(game, adminRole, "KEY-2", 20, DateTime.Now, 0);
            
            Assert.That(Key.Extent.All().Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(Game.ViewAllGames, Does.Contain(game));

            game.DeleteGame();

            Assert.That(Game.ViewAllGames, Does.Not.Contain(game));
            Assert.That(Key.Extent.All(), Does.Not.Contain(key1));
            Assert.That(Key.Extent.All(), Does.Not.Contain(key2));
        }

        [Test]
        public void DeleteKey_ShouldRemoveFromGame()
        {
            var adminEmp = new Employee(
                new Name("Big", "Tommy"), 
                "big.tommy4@example.com", 
                "+48123456789", 
                "pass", 
                5000,
                null
            );
            var adminRole = adminEmp.AssignAdminRole();

            var publisher = new Publisher("Valve", "PC Platform", adminRole);
            var game = new Game("Portal 2", "Puzzle", publisher, adminRole);
            var key = new RegularKey(game, adminRole, "P2-KEY", 10, DateTime.Now, 0);
            
            key.DeleteKey();

            Assert.That(Key.Extent.All(), Does.Not.Contain(key));
            Assert.That(game.Keys, Does.Not.Contain(key));
            Assert.That(game.Keys, Is.Empty);
            Assert.That(Game.ViewAllGames, Does.Contain(game));
        }
    }
}