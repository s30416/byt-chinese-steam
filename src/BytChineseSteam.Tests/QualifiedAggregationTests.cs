using BytChineseSteam.Models;

namespace BytChineseSteam.Tests;

public class QualifiedAggregationTests
{
    private Category _categoryA;
    private Category _categoryB;
    private Game _game1;
    private Game _game2;

    [SetUp]
    public void Setup()
    {
        _categoryA = new Category("Action");
        _categoryB = new Category("Adventure");
        _game1 = new Game("Game One", "Desc1", new Publisher("Pub1", "descr"));
        _game2 = new Game("Game Two", "Desc2", new Publisher("Pub2", "descr"));
    }

    [Test]
    public void GameCanExistWithoutCategory()
    {
        var game = new Game("Lonely Game", "No Category", new Publisher("PubX", "descr"));

        // _game exists even if not in any category
        Assert.That(game.GetAllCategoriesForGame().Count, Is.EqualTo(0));
        Assert.That(game, Is.Not.Null);
    }

    [Test]
    public void AddGame_ShouldUpdateBothSides()
    {
        _categoryA.AddGame(_game1);

        // normal behaviour check (both category dictionary and _game set are updated)
        Assert.That(_categoryA.GetAllGamesInCategory, Does.Contain(_game1));
        Assert.That(_game1.GetAllCategoriesForGame, Does.Contain(_categoryA));
    }

    [Test]
    public void RemoveGameFromCategory_ShouldNotDeleteGame()
    {
        _categoryA.AddGame(_game1);
        _categoryA.RemoveGame(_game1);

        // _game exists after removing it from the category (aggregation)
        Assert.That(_game1, Is.Not.Null);

        // reference checks
        Assert.That(_game1.GetAllCategoriesForGame, Does.Not.Contain(_categoryA));
        Assert.That(_categoryA.GetAllGamesInCategory, Does.Not.Contain(_game1));
    }

    [Test]
    public void GameCanBelongToMultipleCategories()
    {
        _categoryA.AddGame(_game1);
        _categoryB.AddGame(_game1);
        
        // reference checks for multiple categories
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

        // reference checks: not influencing the other categories
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

        // simulate removing the whole Category
        foreach (var game in _categoryA.GetAllGamesInCategory)
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
}