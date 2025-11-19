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
