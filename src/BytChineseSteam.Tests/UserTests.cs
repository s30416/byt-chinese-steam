using System.ComponentModel.DataAnnotations;
using BytChineseSteam.Models;

namespace BytChineseSteam.Tests
{
    public class UserTests
    {
        
        // Because of the fact, that my User is abstract I did this
        private class TestUser : User
        {
            public TestUser(Name name, string email, string phoneNumber, string hashedPassword)
                : base(name, email, phoneNumber, hashedPassword)
            {
            }

            public TestUser() : base(
                new Name("valid", "user"),
                $"valid_{Guid.NewGuid()}@example.com",
                "1234567890",
                "password123"
            )
            {
            }
        }

        
        // Because of the fact, that my validation rules - it's attributes and tests do not have any framework (isolated environment),
        // which can read them and start those rules (ASP.NET have). I created my own validator to check my rules.
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, context, validationResults, validateAllProperties: true);
            return validationResults;
        }
        
        
        [SetUp]
        public void Setup()
        {
            var allUsers = User.ViewAllUsers().ToList();
            foreach (var user in allUsers)
            {
                User.DeleteUser(user.Email);
            }
        }

        // Tests for class Name
        
        [Test]
        public void Name_WhenFirstNameIsNull_ShouldBeInvalid()
        {
            var name = new Name { LastName = "One" };

            var errors = ValidateModel(name);

            Assert.That(errors, Is.Not.Empty);
            Assert.That(errors.Any(e => e.MemberNames.Contains("FirstName")), Is.True);
        }

        [Test]
        public void Name_WhenLastNameIsNull_ShouldBeInvalid()
        {
            var name = new Name { FirstName = "Some" };

            var errors = ValidateModel(name);

            Assert.That(errors, Is.Not.Empty);
            Assert.That(errors.Any(e => e.MemberNames.Contains("LastName")), Is.True);
        }

        [Test]
        public void Name_WhenAllPropertiesAreValid_ShouldBeValid()
        {
            var name = new Name { FirstName = "Olek", LastName = "Ovdikos" };

            var errors = ValidateModel(name);

            Assert.That(errors, Is.Empty);
        }
        
        // Tests for class User
        
        [Test]
        public void User_WhenEmailIsNull_ShouldBeInvalid()
        {
            Assert.Throws<ValidationException>(() => new TestUser(
                new Name("Test", "User"), 
                null!,
                "1234567890", 
                "asdasdafg"
            ));
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
                Name = new Name { FirstName = "Dgfhdfh", LastName = "DFdsfkjd" },
                Email = "dfgdfg@example.com",
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
                Name = new Name { FirstName = "Dfkjdfbldf", LastName = "DFfbdkf" },
                Email = "valid.email@example.com",
                PhoneNumber = "+123-456-7890",
                HashedPassword = "some_hash"
            };
            
            var errors = ValidateModel(user);

            Assert.That(errors, Is.Empty);
        }
        

        // CRUD tests for User
        
        [Test]
        public void CreateUser_WhenUserIsNew_ShouldAddUserToList()
        {
            var user = new TestUser(new Name("Some", "One"), "someone1@example.com", "+1234567890", "hash1dfgdsf");
            
            // User.CreateUser(user);

            var users = User.ViewAllUsers();
            Assert.That(users.Count, Is.EqualTo(1));
            Assert.That(users[0].Email, Is.EqualTo("someone1@example.com"));
        }

        [Test]
        public void CreateUser_WhenEmailExists_ShouldThrowArgumentException()
        {
            var user1 = new TestUser(new Name("Some", "One"), "Some@example1.com", "+1234567890", "hash1dfgdsf");
            // User.CreateUser(user1);
            
            var user2 = new TestUser(new Name("Wone", "One"), "Some@example1.com", "+0987654321", "hash1dfgdsf");

            Assert.Throws<ArgumentException>(() => User.CreateUser(user2));
        }

        [Test]
        public void GetUserByEmail_WhenUserExists_ShouldReturnUser()
        {
            var user = new TestUser(new Name("Some", "One"), "Some@example1.com", "+1234567890", "hashasdasdwqe");
            // User.CreateUser(user);

            var foundUser = User.GetUserByEmail("Some@example1.com");

            Assert.That(foundUser, Is.Not.Null);
            Assert.That(foundUser.Name.FirstName, Is.EqualTo("Some"));
        }

        [Test]
        public void GetUserByEmail_WhenUserOnesNotExist_ShouldReturnNull()
        {
            var foundUser = User.GetUserByEmail("missing@example.com");
            Assert.That(foundUser, Is.Null);
        }
        
        [Test]
        public void UpdateUser_WhenUserExists_ShouldUpdateDetails()
        {
            var user = new TestUser(new Name("Some", "One"), "Some@example.com", "+1234567890", "hashasdasdadw");
            // User.CreateUser(user);

            var newName = new Name("Some", "One");
            var newPhone = "+1112223333";
            
            var updatedUser = User.UpdateUser("Some@example.com", newName, newPhone);

            Assert.That(updatedUser, Is.Not.Null);
            Assert.That(updatedUser.Name.LastName, Is.EqualTo("One"));
            Assert.That(updatedUser.PhoneNumber, Is.EqualTo(newPhone));
            
            var userFromDb = User.GetUserByEmail("Some@example.com");
            Assert.That(userFromDb.Name.LastName, Is.EqualTo("One"));
        }

        [Test]
        public void UpdateUser_WhenUserOnesNotExist_ShouldReturnNull()
        {
            var updatedUser = User.UpdateUser("missing@example.com", new Name("A", "B"), "123");
            Assert.That(updatedUser, Is.Null);
        }

        [Test]
        public void DeleteUser_WhenUserExists_ShouldReturnTrueAndRemoveUser()
        {
            var user = new TestUser(new Name("Some", "One"), "Some@example1.com", "+1234567890", "hash1dfgdsf");
            // User.CreateUser(user);
            
            Assert.That(User.ViewAllUsers().Count, Is.EqualTo(1));

            bool result = User.DeleteUser("Some@example1.com");

            Assert.That(result, Is.True);
            Assert.That(User.ViewAllUsers().Count, Is.EqualTo(0));
        }
        
        [Test]
        public void DeleteUser_WhenUserOnesNotExist_ShouldReturnFalse()
        {
            bool result = User.DeleteUser("missing@example.com");
            Assert.That(result, Is.False);
        }
        
    }
}