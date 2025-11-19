
using System.Diagnostics;
using BytChineseSteam.Models;
using BytChineseSteam.Repository.Extent;
using NUnit.Framework.Legacy;

namespace BytChineseSteam.Tests;

public class ExtentTest
{
    [Test]
    public void Extent_ShouldBeEmptyList_OnStart()
    {
        Assert.That(new List<Key>(), Is.EquivalentTo(Key.Extent.All()));
    }

    [Test]
    public void Extent_ShouldBeEmptyList_OnDeserialization()
    {
        ExtentPersistence.LoadAll();
        Assert.That(new List<Key>(), Is.EquivalentTo(Key.Extent.All()));
    }
}
