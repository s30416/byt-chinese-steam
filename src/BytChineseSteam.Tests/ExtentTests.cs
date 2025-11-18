using BytChineseSteam.Models;
using BytChineseSteam.Repository.Utils;

namespace BytChineseSteam.Tests;

public class ExtentPersistencyServiceTests
{
    private const string StoragePath =
        "C:\\Users\\Andrii Sysoiev\\RiderProjects\\byt-chinese-steam\\src\\BytChineseSteam.Repository\\Storage\\storage.json";

    [SetUp]
    public void SetUp()
    {
        // clear extent before each test
        ClearExtents();
        
        // clean save file
        if (File.Exists(StoragePath))
            File.Delete(StoragePath);
    }

    [Test]
    public async Task SaveFilesAsync_ShouldCreateStorageFile()
    {
        await ExtentPersistencyService.SaveFilesAsync();
        
        Assert.That(File.Exists(StoragePath), Is.True,
            "SaveFilesAsync should create the storage file.");
    }

    [Test]
    public async Task LoadObjectsAsync_ShouldRestoreExtents()
    {
        // create
        var admin = Employee.CreateEmployee<Admin>("Alice", "Brown", 1000, true);
        var manager = Employee.CreateEmployee<Manager>("John", "Smith", 500, true);

        // save
        await ExtentPersistencyService.SaveFilesAsync();

        // simulate restart
        ClearExtents();

        // load
        await ExtentPersistencyService.LoadObjectsAsync();

        // check Admin extent
        var admins = Admin.ViewAllAdmins();
        Assert.That(admins, Is.Not.Null);
        Assert.That(admins.Count, Is.EqualTo(1));
        Assert.That(admins.Any(a => a.Name.FirstName == "Alice"), Is.True);
        
        
        // check Manager extent
        var managers = Manager.ViewAllManagers();
        Assert.That(managers, Is.Not.Null);
        Assert.That(managers.Count, Is.EqualTo(1));
        Assert.That(managers.Any(m => m.Name.FirstName == "John"), Is.True);
        
        // check Employee extent
        var employees = Employee.ViewAllEmployees();
        Assert.That(employees, Is.Not.Null);
        Assert.That(employees.Count, Is.EqualTo(2));
    }
    
    [Test]
    public async Task SaveAndLoad_ShouldHandleMultipleObjectsAcrossExtents()
    {
        // create Admins
        Employee.CreateEmployee<Admin>("A1", "User", 1000, true);
        Employee.CreateEmployee<Admin>("A2", "User", 1100, true);
        Employee.CreateEmployee<Admin>("A3", "User", 1200, true);
        Employee.CreateEmployee<Admin>("A4", "User", 1300, true);
        Employee.CreateEmployee<Admin>("A5", "User", 1400, true);

        // create SuperAdmins
        Employee.CreateEmployee<SuperAdmin>("SA1", "User", 2000, true);
        Employee.CreateEmployee<SuperAdmin>("SA2", "User", 2100, true);
        Employee.CreateEmployee<SuperAdmin>("SA3", "User", 2200, true);

        // create Managers
        Employee.CreateEmployee<Manager>("M1", "User", 500, true);
        Employee.CreateEmployee<Manager>("M2", "User", 550, true);

        // save
        await ExtentPersistencyService.SaveFilesAsync();

        // simulate restart
        ClearExtents();

        // load
        await ExtentPersistencyService.LoadObjectsAsync();

        // check Admin extent
        var admins = Admin.ViewAllAdmins();
        Assert.That(admins.Count, Is.EqualTo(5), "Admin extent should contain 5 entries.");

        // check SuperAdmin extent
        var superAdmins = SuperAdmin.ViewAllSuperAdmins();
        Assert.That(superAdmins.Count, Is.EqualTo(3), "SuperAdmin extent should contain 3 entries.");

        // check Manager extent
        var managers = Manager.ViewAllManagers();
        Assert.That(managers.Count, Is.EqualTo(2), "Manager extent should contain 2 entries.");

        // check Employee extent
        var employees = Employee.ViewAllEmployees();
        Assert.That(employees.Count, Is.EqualTo(10), "Employee extent should contain 10 entries total.");
    }

    private void ClearExtents()
    {
        ClearList(typeof(Admin), "_admins");
        ClearList(typeof(Manager), "_managers");
        ClearList(typeof(Employee), "_employees");
        ClearList(typeof(SuperAdmin), "_superAdmins");
        ClearList(typeof(Publisher), "_publishers");
    }

    // temporarily do this reflection abomination (while we don't have deletion methods)
    private void ClearList(Type t, string field)
    {
        var f = t.GetField(field, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        if (f != null)
        {
            var list = f.GetValue(null) as System.Collections.IList;
            list?.Clear();
        }
    }
}
