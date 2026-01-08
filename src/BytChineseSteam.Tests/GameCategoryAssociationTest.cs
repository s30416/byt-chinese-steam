using BytChineseSteam.Models;
using BytChineseSteam.Models.Interfaces;

namespace BytChineseSteam.Tests;

public class GameCategoryAssociationTest
{
    private Category _categoryA;
    private Category _categoryB;
    private Game _game1;
    private Game _game2;
    private IAdmin _adminRole;
    
    private Employee _adminEmployee;

    [SetUp]
    public void Setup()
    {
        _adminEmployee = new Employee(
            new Name("first", "last"), 
            "admil@mail.com",  
            "+48123456789", 
            "passasdf", 
            null
        );
        _adminRole = _adminEmployee.AssignAdminRole();
        
        
        _categoryA = new Category("Action");
        _categoryB = new Category("Adventure");

        // note: no validation for Game.cs or Category.cs is checked here, so these constructors are fine
        var pub1 = new Publisher("Pub1", "descr", _adminRole);
        
        var game1AdminEmp = new Employee(
            new Name("Big", "Tommy"), 
            "big.tommy@example.com", 
            "+48123456789",
            "howdoesourhashedpasswork", 
            null
        );
        var game1AdminRole = game1AdminEmp.AssignAdminRole();

        _game1 = new Game(
            "Game One", 
            "Desc1", 
            pub1,
            game1AdminRole
        );

        var pub2 = new Publisher("Pub2", "descr", _adminRole);
        
        var game2AdminEmp = new Employee(
            new Name("Big", "Tommy2"),
            "big.tommy2@example.com", 
            "+48123456789",
            "howdoesourhashedpasswork", 
            null
        );
        var game2AdminRole = game2AdminEmp.AssignAdminRole();

        _game2 = new Game(
            "Game Two", 
            "Desc2", 
            pub2,
            game2AdminRole
        );
    }

    [Test]
    public void GameCanExistWithoutCategory()
    {
        var localAdminEmp = new Employee(
            new Name("Lonely", "Admin"), 
            "lonely@test.com", 
            "+1234567890", 
            "passsdahjsd", 
            null
        );
        var localAdminRole = localAdminEmp.AssignAdminRole();
        var pubX = new Publisher("PubX", "descr", _adminRole);

        var game = new Game(
            "Lonely Game",
            "No Category",
            pubX,
            localAdminRole
        );

        // _game exists even if not in any category
        Assert.That(game.GetAllCategoriesForGame().Count, Is.EqualTo(0));
        Assert.That(game, Is.Not.Null);
    }

    [Test]
    public void AddGame_ShouldUpdateBothSides()
    {
        _categoryA.AddGame(_game1);

        Assert.That(_categoryA.GetAllGamesInCategory, Does.Contain(_game1));
        Assert.That(_game1.GetAllCategoriesForGame, Does.Contain(_categoryA));
    }

    [Test]
    public void AddCategory_FromGameSide_ShouldUpdateBothSides()
    {
        _game1.AddCategory(_categoryA);

        Assert.That(_categoryA.GetAllGamesInCategory, Does.Contain(_game1));
        Assert.That(_game1.GetAllCategoriesForGame, Does.Contain(_categoryA));
    }

    [Test]
    public void RemoveGameFromCategory_ShouldNotDeleteGame()
    {
        _categoryA.AddGame(_game1);
        _categoryA.RemoveGame(_game1);

        Assert.That(_game1, Is.Not.Null);
        Assert.That(_game1.GetAllCategoriesForGame, Does.Not.Contain(_categoryA));
        Assert.That(_categoryA.GetAllGamesInCategory, Does.Not.Contain(_game1));
    }

    [Test]
    public void RemoveCategory_FromGameSide_ShouldUpdateBothSides()
    {
        _categoryA.AddGame(_game1);

        _game1.RemoveCategory(_categoryA);

        Assert.That(_categoryA.GetAllGamesInCategory, Does.Not.Contain(_game1));
        Assert.That(_game1.GetAllCategoriesForGame, Does.Not.Contain(_categoryA));
    }

    [Test]
    public void GameCanBelongToMultipleCategories()
    {
        _categoryA.AddGame(_game1);
        _categoryB.AddGame(_game1);

        Assert.That(_game1.GetAllCategoriesForGame, Does.Contain(_categoryA));
        Assert.That(_game1.GetAllCategoriesForGame, Does.Contain(_categoryB));

        Assert.That(_categoryA.GetAllGamesInCategory, Does.Contain(_game1));
        Assert.That(_categoryB.GetAllGamesInCategory, Does.Contain(_game1));
    }

    [Test]
    public void RemoveGameFromOneCategory_DoesNotAffectOtherCategories()
    {
        _categoryA.AddGame(_game1);
        _categoryB.AddGame(_game1);

        _categoryA.RemoveGame(_game1);

        Assert.That(_categoryA.GetAllGamesInCategory, Does.Not.Contain(_game1));
        Assert.That(_categoryB.GetAllGamesInCategory, Does.Contain(_game1));

        Assert.That(_game1.GetAllCategoriesForGame, Does.Contain(_categoryB));
        Assert.That(_game1.GetAllCategoriesForGame, Does.Not.Contain(_categoryA));

        // _game still exists
        Assert.That(_game1, Is.Not.Null);
    }

    [Test]
    public void RemovingCategory_ShouldUpdateGames()
    {
        _categoryA.AddGame(_game1);
        _categoryA.AddGame(_game2);

        foreach (var game in _categoryA.GetAllGamesInCategory.ToList())
        {
            _categoryA.RemoveGame(game);
        }

        Assert.That(_categoryA.GetAllGamesInCategory.Count, Is.EqualTo(0));
        Assert.That(_game1.GetAllCategoriesForGame, Does.Not.Contain(_categoryA));
        Assert.That(_game2.GetAllCategoriesForGame, Does.Not.Contain(_categoryA));

        // games still exist
        Assert.That(_game1, Is.Not.Null);
        Assert.That(_game2, Is.Not.Null);
    }

    [Test]
    public void AddingSameGameTwice_DoesNotDuplicate()
    {
        _categoryA.AddGame(_game1);
        _categoryA.AddGame(_game1);

        Assert.That(_categoryA.GetAllGamesInCategory.Count, Is.EqualTo(1));
        Assert.That(_game1.GetAllCategoriesForGame().Count, Is.EqualTo(1));
    }

    [Test]
    public void AddingSameCategoryTwice_FromGameSide_IsIdempotent()
    {
        _game1.AddCategory(_categoryA);
        _game1.AddCategory(_categoryA);

        Assert.That(_categoryA.GetAllGamesInCategory.Count, Is.EqualTo(1));
        Assert.That(_game1.GetAllCategoriesForGame().Count, Is.EqualTo(1));
    }
}
