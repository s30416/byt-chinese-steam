using System.Collections;
using System.Reflection;
using BytChineseSteam.Models;
using BytChineseSteam.Models.Interfaces;

namespace BytChineseSteam.Tests
{
    [TestFixture]
    public class PublisherTests
    {
        // these are set up fresh in SetUp()
        private Publisher _pubA;
        private Publisher _pubB;
        private IAdmin _adminRole;
        private IManager _managerRole;
        
        private Employee _adminEmployee;
        private Employee _managerEmployee;

        [SetUp]
        public void SetUp()
        {
            ClearPublisherStaticList();
            ClearGameStaticList();
            
            
            _managerEmployee = new Employee(
                new Name("first", "last"), 
                "manager@gmail.com", 
                "+48123456789", 
                "asdfasdfasdf", 
                5000, 
                null
            );
            _managerRole = _managerEmployee.AssignManagerRole();
            
            
            _adminEmployee = new Employee(
                new Name("first", "last"), 
                "admin@gmail.com", 
                "+48123456789", 
                "asdfasdfasdf", 
                5000, 
                null
            );
            _adminRole = _adminEmployee.AssignAdminRole();
            
            // create two publishers for tests that need them
            _pubA = Publisher.CreatePublisher("PubA", "Desc A", _adminEmployee);
            _pubB = Publisher.CreatePublisher("PubB", "Desc B", _adminEmployee);
        }

        [TearDown]
        public void TearDown()
        {
            ClearPublisherStaticList();
            ClearGameStaticList();
        }

        private static void ClearPublisherStaticList()
        {
            var publisherType = typeof(Publisher);

            // Get the static Extent field
            var extentField = publisherType.GetField("Extent", BindingFlags.NonPublic | BindingFlags.Static);
            if (extentField == null) throw new InvalidOperationException("Could not find field Extent via reflection.");

            // Get the instance of Extent<Publisher>
            var extentInstance = extentField.GetValue(null);
            if (extentInstance == null) return;

            // Get the private _items list inside that instance
            var extentType = extentInstance.GetType();
            var itemsField = extentType.GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
            if (itemsField == null)
                throw new InvalidOperationException("Could not find field _items inside Extent via reflection.");

            // Clear list.
            var list = (IList)itemsField.GetValue(extentInstance)!;
            list?.Clear();
        }

        private static void ClearGameStaticList()
        {
            var gameType = typeof(Game);
            var field = gameType.GetField("_viewAllGames", BindingFlags.NonPublic | BindingFlags.Static);
            // Game implementation will change, keep this check loose 
            if (field == null) return;
            var list = (IList)field.GetValue(null)!;
            list?.Clear();
        }

        [Test]
        public void TestCreatePublisher_AddsToExtent()
        {
            var countBefore = Publisher.GetAll().Count;
            var newPub = Publisher.CreatePublisher("NewPub", "NewDesc", _adminEmployee);

            Assert.That(Publisher.GetAll().Count, Is.EqualTo(countBefore + 1));
            Assert.That(Publisher.GetAll().Contains(newPub), Is.True);
        }

        [Test]
        public void TestCreatePublisher_NotAdmin_ThrowsException()
        {
            Assert.Throws<UnauthorizedAccessException>(() =>
                Publisher.CreatePublisher("Barack Obama", "Let me be clear", _adminEmployee));
        }

        [Test]
        public void TestReadPublisher_CanRetrieveByName()
        {
            // Assuming we use GetAll() to find it, as implemented in the class
            var foundPub = Publisher.GetAll().FirstOrDefault(p => p.Name == "PubA");
            Assert.That(foundPub, Is.Not.Null);
            Assert.That(foundPub.Description, Is.EqualTo("Desc A"));
        }

        [Test]
        public void TestUpdatePublisher_UpdatesProperties()
        {
            _pubA.UpdatePublisher("PubA_Updated", "Desc A_Updated", _adminEmployee);

            Assert.That(_pubA.Name, Is.EqualTo("PubA_Updated"));
            Assert.That(_pubA.Description, Is.EqualTo("Desc A_Updated"));

            // Check if change is reflected in the global list
            var storedPub = Publisher.GetAll().FirstOrDefault(p => p.Name == "PubA_Updated");
            Assert.That(storedPub, Is.Not.Null);
            Assert.That(storedPub.Description, Is.EqualTo("Desc A_Updated"));
        }

        [Test]
        public void TestDeletePublisher_RemovesFromExtent()
        {
            Publisher.DeletePublisher("PubA", _adminEmployee);

            var storedPub = Publisher.GetAll().FirstOrDefault(p => p.Name == "PubA");
            Assert.That(storedPub, Is.Null);
        }

        [Test]
        public void TestDeletePublisher_NotFound_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                Publisher.DeletePublisher("NonExistentPub", _adminEmployee));
        }

        [Test]
        public void TestGetAllPublishersGames_ReturnsOnlyGamesWithThisPublisher()
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
            
            var g1 = new Game("G1", "descr1", _pubA, (Admin)adminRole);
            var g2 = new Game("G2", "descr", _pubB, (Admin)adminRole);
            
            var aGames = _pubA.GetAllPublishersGames();
            var bGames = _pubB.GetAllPublishersGames();
            Assert.Multiple(() =>
            {
                Assert.That(aGames, Has.Count.EqualTo(1));
                Assert.That(aGames[0].Title, Is.EqualTo("G1"));
                Assert.That(bGames, Has.Count.EqualTo(1));
                Assert.That(bGames[0].Title, Is.EqualTo("G2"));
            });
        }
    }
}