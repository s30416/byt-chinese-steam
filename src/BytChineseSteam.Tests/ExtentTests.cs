using BytChineseSteam.Models;
using BytChineseSteam.Repository.Utils;

namespace BytChineseSteam.Tests
{
    [TestFixture]
    public class ExtentServiceTests
    {
        [Test]
        public async Task SaveObjectAsync_ShouldReturnTrue_WhenSavingList()
        {
            // Arrange
            var publishers = new List<Publisher>
            {
                Publisher.CreatePublisher("TestPub", "Just a test", true)
            };

            // Act
            var result = await ExtentService.SaveObjectAsync<List<Publisher>>(publishers);

            // Assert
            Assert.That(result, Is.True, "SaveObjectAsync should return true for a successful save.");
        }

        [Test]
        public async Task LoadObjectAsync_ShouldReturnSavedObjects()
        {
            // Arrange
            var publishers = new List<Publisher>
            {
                Publisher.CreatePublisher("LoadTestPub", "Testing load", true)
            };

            await ExtentService.SaveObjectAsync<List<Publisher>>(publishers);

            // Act
            var loaded = await ExtentService.LoadObjectAsync<List<Publisher>>();

            // Assert
            Assert.That(loaded, Is.Not.Null, "LoadObjectAsync should not return null.");
            Assert.That(loaded, Is.Not.Empty, "Loaded list should contain at least one item.");
            Assert.That(loaded[0].Name, Is.EqualTo("LoadTestPub"), "Publisher name should match saved data.");
        }
    }
}