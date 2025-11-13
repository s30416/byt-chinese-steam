using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models;

namespace BytChineseSteam.Tests
{
    public class UserTests
    {
        
        // Because of the fact, that my User is abstract I did this
        private class TestUser : User { }

        
        // Because of the fact, that my validation rules - it's attributes and tests do not have any framework (isolated environment),
        // which can read them and start those rules (ASP.NET have). I created my own validator to check my rules.
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, context, validationResults, validateAllProperties: true);
            return validationResults;
        }

        // Tests for class Name
        
        [Test]
        public void Name_WhenFirstNameIsNull_ShouldBeInvalid()
        {
            var name = new Name { LastName = "Smith" };

            var errors = ValidateModel(name);

            Assert.That(errors, Is.Not.Empty);
            Assert.That(errors.Any(e => e.MemberNames.Contains("FirstName")), Is.True);
        }

        [Test]
        public void Name_WhenLastNameIsNull_ShouldBeInvalid()
        {
            var name = new Name { FirstName = "John" };

            var errors = ValidateModel(name);

            Assert.That(errors, Is.Not.Empty);
            Assert.That(errors.Any(e => e.MemberNames.Contains("LastName")), Is.True);
        }

        [Test]
        public void Name_WhenAllPropertiesAreValid_ShouldBeValid()
        {
            var name = new Name { FirstName = "John", LastName = "Smith" };

            var errors = ValidateModel(name);

            Assert.That(errors, Is.Empty);
        }
        
        // Tests for class User
        
        [Test]
        public void User_WhenEmailIsNull_ShouldBeInvalid()
        {
            var user = new TestUser
            {
                Name = new Name { FirstName = "Test", LastName = "User" },
                Email = null
            };
            
            var errors = ValidateModel(user);

            Assert.That(errors, Is.Not.Empty);
            Assert.That(errors.Any(e => e.MemberNames.Contains("Email")), Is.True);
        }

        [Test]
        public void User_WhenEmailIsInvalid_ShouldBeInvalid()
        {
            var user = new TestUser
            {
                Name = new Name { FirstName = "Test", LastName = "User" },
                Email = "not-a-valid-email"
            };
            
            var errors = ValidateModel(user);

            Assert.That(errors, Is.Not.Empty);
            Assert.That(errors.Any(e => e.MemberNames.Contains("Email")), Is.True);
        }
        
        [Test]
        public void User_WhenPhoneNumberIsInvalid_ShouldBeInvalid()
        {
            var user = new TestUser
            {
                Name = new Name { FirstName = "Test", LastName = "User" },
                Email = "test@example.com",
                PhoneNumber = "12345"
            };
            
            var errors = ValidateModel(user);

            Assert.That(errors, Is.Not.Empty);
            Assert.That(errors.Any(e => e.MemberNames.Contains("PhoneNumber")), Is.True);
        }

        [Test]
        public void User_WhenAllPropertiesAreValid_ShouldBeValid()
        {
            var user = new TestUser
            {
                Name = new Name { FirstName = "Test", LastName = "User" },
                Email = "valid.email@example.com",
                PhoneNumber = "+123-456-7890",
                HashedPassword = "some_hash"
            };
            
            var errors = ValidateModel(user);

            Assert.That(errors, Is.Empty);
        }
    }
}