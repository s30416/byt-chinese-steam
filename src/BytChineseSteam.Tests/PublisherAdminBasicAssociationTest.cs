using BytChineseSteam.Models;

namespace BytChineseSteam.Tests;

public class PublisherAdminBasicAssociationTest
{
    private Admin CreateAdmin()
    {
        var admin = new Admin(new User(new Name("first", "last"), "admin@mail.com", "+11111111111", "password"), null);
        Console.WriteLine(admin.GetHashCode());
        return admin;
    }

    private Publisher CreatePublisher(Admin admin, string name="name")
    {
        return new Publisher(name, "description", admin);
    }

    [Test]
    public void Admin_ShouldContainEmptyPublishers_WhenCreated()
    {
        var admin = CreateAdmin();
        Assert.That(admin.Publishers, Is.Empty);
    }

    [Test]
    public void Admin_ShouldContainPublisher_OnPublisherConstructor()
    {
        var admin = CreateAdmin();
        var publisher = CreatePublisher(admin);
        
        Assert.That(admin.Publishers, Contains.Item(publisher));
    }

    [Test]
    public void Admin_ShouldNotContainPublisher_OnPublisherDeletePublisher()
    {
        var admin = CreateAdmin();
        var publisher = CreatePublisher(admin, "test3");
        Assert.That(admin.Publishers, Contains.Item(publisher));
        
        Publisher.DeletePublisher(publisher.Name, admin);
        Assert.That(admin.Publishers, Does.Not.Contain(publisher));
    }
}