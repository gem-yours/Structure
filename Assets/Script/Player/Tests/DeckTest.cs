using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

#nullable enable
public class DeckTest
{
#pragma warning disable CS8618
    private Deck deck;
    private List<Spell> spells;
#pragma warning restore CS8618

    [SetUp]
    public void OneTimeSetup()
    {
        spells = new List<Spell> {
            new Ignis(),
            new Explosion(),
            new Ignis()
        };
        deck = new Deck(spells, 0, 0);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void CheckDrawPile()
    {
        Assert.AreEqual(deck.drawPile, spells);
    }

    [Test]
    public void CheckDiscardPile()
    {

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator DeckTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
