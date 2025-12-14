using BytChineseSteam.Models;

namespace BytChineseSteam.Tests;

public class SuperAdminEmployeeAssociationTests
{
    private SuperAdmin _superAdmin;

    [SetUp]
    public void Setup()
    {
        
        _superAdmin = new SuperAdmin(
            new User(new Name("Super", "Boss"), 
                "boss@company.com", 
                "+48123456789", 
                "hasdashjdbasdad"), 
            10000
        );
    }

    [Test]
    public void ShouldEstablishReverseConnection_WhenCreatingAdminWithCreator()
    {
        var admin = new Admin(
            new User(new Name("Sub", "Admin"), 
                "admin@company.com", 
                "+48123456789", 
                "hasdashjdbasdad"), 
            5000, 
            creator: _superAdmin
        );

        Assert.That(admin.Creator, Is.EqualTo(_superAdmin));
        
        Assert.That(_superAdmin.CreatedEmployees, Does.Contain(admin));
        Assert.That(_superAdmin.CreatedEmployees.Count, Is.EqualTo(1));
    }

    [Test]
    public void ShouldEstablishReverseConnection_WhenUsingFactoryMethod()
    {
        var employee = Employee.CreateEmployee<Manager>(
            "Manager", "One", 
            "manager@company.com", 
            "+48123456789", 
            "hasdashjdbasdad", 
            4000, 
            _superAdmin
        );

        Assert.That(employee, Is.InstanceOf<Manager>());
        Assert.That(employee.Creator, Is.EqualTo(_superAdmin));
        Assert.That(_superAdmin.CreatedEmployees, Does.Contain(employee));
    }

    [Test]
    public void ShouldAllowNullCreator_ForRootSuperAdmin()
    {
        var anotherSuper = new SuperAdmin(
            new User(new Name("Another", "Boss"), 
                "founder@company.com", 
                "+48123456789", 
                "hasdashjdbasdad"), 
            null
        );

        Assert.That(anotherSuper.Creator, Is.Null);
    }

    [Test]
    public void ShouldThrowException_WhenAddingEmployeeCreatedByAnotherAdmin()
    {
        var otherSuperAdmin = new SuperAdmin(new User(new Name("Other", "Guy"), "random@b.com", "+48123456789", "hasdashjdbasdad"), 0);
        
        var employee = new Admin(new User(new Name("A", "B"), "random@b.com", "+48123456789", "hasdashjdbasdad"), 0, _superAdmin);

        Assert.Throws<InvalidOperationException>(() => 
            otherSuperAdmin.AddCreatedEmployee(employee)
        );
    }
}