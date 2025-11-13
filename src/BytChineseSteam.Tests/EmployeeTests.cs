using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models;

namespace BytChineseSteam.Tests
{
    [TestFixture]
    public class EmployeeTests
    {
        private static void ValidateModel(object model)
        {
            var context = new ValidationContext(model);
    
            Validator.ValidateObject(model, context, true);
        }
        
        [Test]
        public void CreateEmployee_ShouldFailValidation_WhenSuperAdminTrue()
        {
            var manager = Employee.CreateEmployee<Manager>(
                "emp1", "manager", "whatever", "1234567890", "hashedpwd", 5000m, true);

            Assert.Throws<ValidationException>(() =>  ValidateModel(manager));
        }

        [Test]
        public void CreateEmployee_ShouldThrow_WhenNotSuperAdmin()
        {
            Assert.That(() => 
                Employee.CreateEmployee<Admin>(
                    "emp2", "admin", "emp2@example.com", "9876543210", "hashedpwd", 4000m, false),
                Throws.TypeOf<UnauthorizedAccessException>());
        }

        [Test]
        public void UpdateEmployee_ShouldUpdateSalaryAndBonuses_WhenSuperAdminTrue()
        {
            var manager = Employee.CreateEmployee<Manager>(
                "emp3", "manager", "emp3@example.com", "1111111111", "hashedpwd", 3500m, true);

            var result = manager.UpdateEmployee("mark@example.com", 4000m, 250m, true);

            Assert.That(result, Is.True);
            Assert.That(manager.Salary, Is.EqualTo(4000m));
            Assert.That(manager.CollectedBonuses, Is.EqualTo(250m));
        }

        [Test]
        public void UpdateEmployee_ShouldThrow_WhenNotSuperAdmin()
        {
            var admin = Employee.CreateEmployee<Admin>(
                "emp4", "admin", "emp4@example.com", "2222222222", "hashedpwd", 4500m, true);

            Assert.That(() =>
                admin.UpdateEmployee("sarah@example.com", 4700m, 200m, false),
                Throws.TypeOf<UnauthorizedAccessException>());
        }

        [Test]
        public void DeleteEmployee_ShouldSucceed_WhenSuperAdminTrue()
        {
            var superAdmin = Employee.CreateEmployee<SuperAdmin>(
                "emp5", "superadmin", "emp5@example.com", "3333333333", "hashedpwd", 6000m, true);

            var deleted = superAdmin.DeleteEmployee(true);

            Assert.That(deleted, Is.True);
        }
    }
}
