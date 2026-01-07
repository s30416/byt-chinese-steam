using BytChineseSteam.Models;
using NUnit.Framework;

namespace BytChineseSteam.Tests;

public class SuperAdminEmployeeAssociationTests
{
    private SuperAdmin _superAdminRole;
    private Employee _superAdminEmployee;

    [SetUp]
    public void Setup()
    {
        _superAdminEmployee = new Employee(
            new Name("Super", "Boss"), 
            "boss@company.com", 
            "+48123456789", 
            "hasdashjdbasdad", 
            10000,
            creator: null
        );

        _superAdminRole = new SuperAdmin(_superAdminEmployee);
    }

    [Test]
    public void ShouldEstablishReverseConnection_WhenCreatingAdminWithCreator()
    {
        var adminEmployee = Employee.CreateEmployee<Admin>(
            "Sub", "Admin", 
            "admin@company.com", 
            "+48123456789", 
            "hasdashjdbasdad", 
            5000, 
            creator: _superAdminRole
        );

        Assert.That(adminEmployee.AdminRole, Is.Not.Null);
        
        Assert.That(adminEmployee.Creator, Is.EqualTo(_superAdminRole));
        
        Assert.That(_superAdminRole.CreatedEmployees, Does.Contain(adminEmployee));
        Assert.That(_superAdminRole.CreatedEmployees.Count, Is.EqualTo(1));
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
            _superAdminRole
        );

        Assert.That(employee.ManagerRole, Is.Not.Null); 
        Assert.That(employee.AdminRole, Is.Null);

        Assert.That(employee.Creator, Is.EqualTo(_superAdminRole));
        Assert.That(_superAdminRole.CreatedEmployees, Does.Contain(employee));
    }

    [Test]
    public void ShouldAllowNullCreator_ForRootSuperAdmin()
    {
        var anotherSuperEmployee = new Employee(
            new Name("Another", "Boss"), 
            "founder@company.com", 
            "+48123456789", 
            "hasdashjdbasdad", 
            null,
            creator: null
        );
        var anotherSuperRole = new SuperAdmin(anotherSuperEmployee);

        Assert.That(anotherSuperEmployee.Creator, Is.Null);
        
        Assert.That(anotherSuperEmployee.SuperAdminRole, Is.EqualTo(anotherSuperRole));
    }

    [Test]
    public void ShouldThrowException_WhenAddingEmployeeCreatedByAnotherAdmin()
    {
        var otherEmp = new Employee(new Name("Other", "Guy"), "random@b.com", "+48123456789", "pass", 0, null);
        var otherSuperAdminRole = new SuperAdmin(otherEmp);
        
        var employee = Employee.CreateEmployee<Admin>("A", "B", "test@b.com", "+48123456789", "pass", 0, _superAdminRole);

        Assert.Throws<InvalidOperationException>(() => 
            otherSuperAdminRole.AddCreatedEmployee(employee)
        );
    }
}