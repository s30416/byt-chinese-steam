using BytChineseSteam.Models;
using BytChineseSteam.Models.Interfaces;

namespace BytChineseSteam.Tests;

public class PublisherAdminBasicAssociationTest
{
    private IAdmin CreateAdmin()
    {
        var emp = new Employee(
            new Name("first", "last"), 
            "admin@mail.com", 
            "+11111111111", 
            "password", 
            null,
            null
        );
        Console.WriteLine(emp.GetHashCode());
        return emp.AssignAdminRole();
    }

    private Publisher CreatePublisher(IAdmin admin, string name="name")
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
        
        var employeeActor = ((Admin)admin).Employee;
        
        Publisher.DeletePublisher(publisher.Name, employeeActor);
        Assert.That(admin.Publishers, Does.Not.Contain(publisher));
    }
}