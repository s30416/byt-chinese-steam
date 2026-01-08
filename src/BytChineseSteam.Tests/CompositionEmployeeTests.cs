using BytChineseSteam.Models;

namespace BytChineseSteam.Tests;

public class CompositionEmployeeTests
{
    private Employee CreateBaseEmployee(string suffix)
    {
        return new Employee(
            new Name("Test", "User" + suffix), 
            $"user{suffix}@byt.com", 
            "+1234567890", 
            "hashedpass", 
            1000, 
            null
        );
    }

    [Test]
    public void ShouldAllowEmployee_ToBeBothAdminAndManager_Overlapping()
    {
        var emp = CreateBaseEmployee("Overlapping");

        var adminRole = emp.AssignAdminRole();
        var managerRole = emp.AssignManagerRole();

        Assert.That(emp.AdminRole, Is.Not.Null);
        Assert.That(emp.ManagerRole, Is.Not.Null);
        
        Assert.That(emp.AdminRole, Is.EqualTo(adminRole));
        Assert.That(emp.ManagerRole, Is.EqualTo(managerRole));
        
        Assert.That(((Admin)adminRole).Employee, Is.EqualTo(emp));
        Assert.That(((Manager)managerRole).Employee, Is.EqualTo(emp));
    }

    [Test]
    public void ShouldAggregateBehavior_GetCollectedBonuses_FromAllRoles()
    {
        var emp = CreateBaseEmployee("Bonuses");
        
        Assert.That(emp.GetCollectedBonuses(), Is.EqualTo(0));

        emp.AssignAdminRole();
        Assert.That(emp.GetCollectedBonuses(), Is.EqualTo(500));

        emp.AssignManagerRole();
        Assert.That(emp.GetCollectedBonuses(), Is.EqualTo(600));
    }

    [Test]
    public void ShouldAllowDynamicRoleRemoval_UnassignRole()
    {
        var emp = CreateBaseEmployee("Dynamic");
        emp.AssignAdminRole();
        emp.AssignManagerRole();

        Assert.That(emp.AdminRole, Is.Not.Null);
        Assert.That(emp.ManagerRole, Is.Not.Null);

        emp.UnassignAdminRole();

        Assert.That(emp.AdminRole, Is.Null);
        Assert.That(emp.ManagerRole, Is.Not.Null);
        
        Assert.That(emp.GetCollectedBonuses(), Is.EqualTo(100));
    }

    [Test]
    public void ShouldPreventDoubleRoleAssignment_ThrowException()
    {
        var emp = CreateBaseEmployee("Double");
        
        var adminRole1 = emp.AssignAdminRole();

        var ex = Assert.Throws<InvalidOperationException>(() => emp.AssignAdminRole());
        
        Assert.That(ex!.Message, Does.Contain("already an Admin"));
    }

    [Test]
    public void ShouldEnforceMandatoryContainer_AdminCannotExistWithoutEmployee()
    {
        Assert.Throws<ArgumentNullException>(() => new Admin(null!));
    }
    
    [Test]
    public void ShouldEnforceMandatoryContainer_ManagerCannotExistWithoutEmployee()
    {
        Assert.Throws<ArgumentNullException>(() => new Manager(null!));
    }
}