using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

#nullable enable
public class DrawManagerTest
{
#pragma warning disable CS8618
    private Deck deck;
    private List<Spell> spells;
    private DrawManager drawManager;
#pragma warning restore CS8618
    private float shuffleTime = 0.02f;

    [SetUp]
    public void Setup()
    {
        spells = new List<Spell> {
            new Ignis(),
            new Explosion(),
            new Ignis(),
            new Ignis()
        };
        deck = new Deck(spells);
        drawManager = new DrawManager(deck, shuffleTime);
    }

    [UnityTest]
    public IEnumerator DrawTest()
    {
        while (deck.canDraw)
        {
            yield return drawManager.Draw(0);
        }
        Assert.AreEqual(spells.Skip(3), deck.drawPile);
        Assert.AreEqual(spells.Take(3), deck.discardPile);
    }

    [UnityTest]
    public IEnumerator CheckAddWhileDraw()
    {
        var newSpell = new Ignis();

        yield return drawManager.Draw(0);
        deck.Add(newSpell);
        yield return drawManager.Draw(0);
        yield return drawManager.Draw(0);

        spells.Add(newSpell);
        Assert.AreEqual(spells.Take(3), deck.discardPile);
        Assert.AreEqual(spells.Skip(3).ToList().Count, deck.drawPile.Count);
    }
}
