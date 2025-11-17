// File: PublisherTests.cs
using System;
using System.Reflection;
using NUnit.Framework;
using BytChineseSteam.Models;
using System.Collections;

namespace BytChineseSteam_Tests
{
    [TestFixture]
    public class PublisherTests
    {
        // these are set up fresh in SetUp()
        private Publisher pubA;
        private Publisher pubB;

        [SetUp]
        public void SetUp()
        {
            ClearPublisherStaticList();
            ClearGameStaticList();

            // create two publishers for tests that need them
            pubA = Publisher.CreatePublisher("PubA", "Desc A", isAdmin: true);
            pubB = Publisher.CreatePublisher("PubB", "Desc B", isAdmin: true);
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
            var field = publisherType.GetField("_publishers", BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null) throw new InvalidOperationException("Could not find field _publishers via reflection.");
            var list = (IList)field.GetValue(null);
            list?.Clear();
        }

        private static void ClearGameStaticList()
        {
            var gameType = typeof(Game); 
            var field = gameType.GetField("_viewAllGames", BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null) throw new InvalidOperationException("Could not find field _viewAllGames via reflection.");
            var list = (IList)field.GetValue(null);
            list?.Clear();
        }

        // ViewAllPublishers
        [Test]
        public void TestViewAllPublishers_ReturnsAllCreatedPublishers()
        {
            var all = Publisher.ViewAllPublishers();
            Assert.That(all.Count, Is.EqualTo(2));
            Assert.That(all[0].Name, Is.EqualTo("PubA"));
            Assert.That(all[1].Name, Is.EqualTo("PubB"));
        }

        // CreatePublisher
        [Test]
        public void TestCreatePublisher_WithAdmin_AddsPublisher()
        {
            var created = Publisher.CreatePublisher("NewPub", "NewDesc", isAdmin: true);
            
            Assert.That(created, Is.Not.Null);
            Assert.That(created.Name, Is.EqualTo("NewPub"));
            Assert.That(created.Description, Is.EqualTo("NewDesc"));

            var all = Publisher.ViewAllPublishers();
            Assert.That(all.Count, Is.EqualTo(3));
            Assert.That(all, Has.Some.Matches<Publisher>(p => p.Name == "NewPub"));
        }

        // DeletePublisher
        [Test]
        public void TestDeletePublisher_RemovesPublisher_FromList()
        {
            Assert.That(Publisher.ViewAllPublishers().Count, Is.EqualTo(2));
            
            Publisher.DeletePublisher("PubA", isAdmin: true);
            
            var all = Publisher.ViewAllPublishers();
            Assert.That(all.Count, Is.EqualTo(1));
            Assert.That(all[0].Name, Is.EqualTo("PubB"));
        }

        // UpdatePublisher (instance method)
        [Test]
        public void TestUpdatePublisher_ChangesNameAndDescription()
        {
            pubA.UpdatePublisher("PubA_New", "Updated description", isAdmin: true);
            
            Assert.That(pubA.Name, Is.EqualTo("PubA_New"));
            Assert.That(pubA.Description, Is.EqualTo("Updated description"));

            var all = Publisher.ViewAllPublishers();
            Assert.That(all[0].Name, Is.EqualTo("PubA_New"));
        }

        // GetAllPublishersGames (instance method)
        [Test]
        public void TestGetAllPublishersGames_ReturnsOnlyGamesWithThisPublisher()
        {
            var g1 = new Game("G1", pubA);
            var g2 = new Game("G2", pubB);
            
            var aGames = pubA.GetAllPublishersGames();
            var bGames = pubB.GetAllPublishersGames();
            
            Assert.That(aGames.Count, Is.EqualTo(1));
            Assert.That(aGames[0].Title, Is.EqualTo("G1"));

            Assert.That(bGames.Count, Is.EqualTo(1));
            Assert.That(bGames[0].Title, Is.EqualTo("G2"));
        }
    }
}
