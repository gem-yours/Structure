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
            new Ignis(),
            new Ignis()
        };
        deck = new Deck(spells);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void CheckDrawPile()
    {
        Assert.AreEqual(spells, deck.drawPile);
    }

    [Test]
    public void CheckDiscardPile()
    {
        for (int i = 0; i < deck.spells.Count; i++)
        {
            var spell = deck.Draw();
            if (spell != null) deck.Discard(spell);
        }
        Assert.AreEqual(spells, deck.discardPile);
    }

    [Test]
    public void CheckIsShuffle()
    {
        for (int i = 0; i < deck.spells.Count; i++)
        {
            var spell = deck.Draw();
            if (spell != null) deck.Discard(spell);
        }
        Assert.True(deck.needShuffle);
    }

    [Test]
    public void CheckCanDraw()
    {
        for (int i = 0; i < 3; i++)
        {
            deck.Draw();
        }
        Assert.False(deck.canDraw);
    }

    [Test]
    public void CheckAddCorrectly()
    {
        var spell = new Ignis();
        deck.AddSpell(spell);
        spells.Add(spell);
        Assert.AreEqual(deck.drawPile, spells);
    }

    [Test]
    public void CanDrawCorrectSpell()
    {
        var spell = deck.Draw();
        spell = spells[0];
    }

    [Test]
    public void CanShuffle()
    {
        for (int i = 0; i < deck.spells.Count; i++)
        {
            var spell = deck.Draw();
            if (spell != null) deck.Discard(spell);
        }
        deck.Shuffle();
        Assert.AreEqual(0, deck.discardPile.Count);
        Assert.AreEqual(spells.Count, deck.drawPile.Count);
    }

    [Test]
    public void CheckOnDraw()
    {
        SpellSlot? slot = null;
        Spell? spell = null;
        deck.onDraw = (SpellSlot sl, Spell sp) =>
        {
            slot = sl;
            spell = sp;
        };
        deck.Draw();
        Assert.AreEqual(slot, SpellSlot.Spell1);
        Assert.AreEqual(spell, spells[0]);
    }

    [Test]
    public void CheckOnAdd()
    {
        Spell? spell = null;
        deck.onAdd = (Deck deck, Spell sl) =>
        {
            spell = sl;
        };
        var newSpell = new Explosion();
        deck.AddSpell(newSpell);
        Assert.AreEqual(newSpell, spell);
        spells.Add(newSpell);
        Assert.AreEqual(spells, deck.drawPile);
    }

    [Test]
    public void CheckOnShuffle()
    {
        var isCalled = false;
        deck.onShuffle = (Deck deck) =>
        {
            isCalled = true;
        };
        deck.Shuffle();
        Assert.True(isCalled);
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
