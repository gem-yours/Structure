using System.Collections;
using System.Collections.Generic;
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
    private float drawTime = 0.01f;
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
        drawManager = new DrawManager(deck, drawTime, shuffleTime);
    }

    [UnityTest]
    public IEnumerator DrawTest()
    {
        // while (deck.canDraw)
        // {
        //     yield return drawManager.Draw();
        // }
        yield return new WaitForSeconds(1);
        Assert.AreEqual(0, deck.drawPile);
        Assert.AreEqual(0, deck.discardPile);
    }
}
