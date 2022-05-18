using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        deck = new Deck(spells, shuffleTime);
    }

    [UnityTest]
    public IEnumerator ContinuouslyDrawTest()
    {
        yield return new MonoBehaviourTest<MonoBehaviourDrawTest>();
    }

    [UnityTest]
    public IEnumerator CheckAddWhileDraw()
    {
        var newSpell = new Ignis();

        yield return deck.Draw();
        deck.Add(newSpell);
        yield return deck.Draw();
        yield return deck.Draw();

        spells.Add(newSpell);
        Assert.AreEqual(spells.Take(3), deck.discardPile);
        Assert.AreEqual(spells.Skip(3).ToList().Count, deck.drawPile.Count);
    }
}


public class MonoBehaviourDrawTest : MonoBehaviour, IMonoBehaviourTest
{
    private Deck deck = new Deck(
        new List<Spell> { new Ignis(), new Ignis(), new Ignis() },
        0f
    );
    public bool IsTestFinished { set; get; } = false;

    private void Start()
    {
        deck.ContinuouslyDraw(this);
    }

    private void Update()
    {
        if (deck.canDraw) return;
        Assert.AreEqual(0, deck.drawPile.Count);
        IsTestFinished = true;
    }
}
