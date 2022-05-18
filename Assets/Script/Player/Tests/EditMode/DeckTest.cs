// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;

// #nullable enable
// public class DeckTest
// {
// #pragma warning disable CS8618
//     private Deck deck;
//     private List<Spell> spells;
// #pragma warning restore CS8618

//     [SetUp]
//     public void Setup()
//     {
//         spells = new List<Spell> {
//             new Ignis(),
//             new Explosion(),
//             new Ignis(),
//             new Ignis()
//         };
//         deck = new Deck(spells);
//     }

//     // A Test behaves as an ordinary method
//     [Test]
//     public void CheckDrawPile()
//     {
//         Assert.AreEqual(spells, deck.drawPile);
//     }

//     [Test]
//     public void CheckDiscardPile()
//     {
//         for (int i = 0; i < deck.spells.Count; i++)
//         {
//             var (spell, _) = deck.Draw();
//             if (spell != null) deck.Use(spell);
//         }
//         Assert.AreEqual(spells, deck.discardPile);
//     }

//     [Test]
//     public void CheckIsShuffle()
//     {
//         for (int i = 0; i < deck.spells.Count; i++)
//         {
//             var (spell, _) = deck.Draw();
//             if (spell != null) deck.Use(spell);
//         }
//         Assert.True(deck.needShuffle);
//     }

//     [Test]
//     public void CheckCanDraw()
//     {
//         for (int i = 0; i < 3; i++)
//         {
//             deck.Draw();
//         }
//         Assert.False(deck.canDraw);
//     }

//     [Test]
//     public void CheckAddCorrectly()
//     {
//         var spell = new Ignis();
//         deck.Add(spell);
//         spells.Add(spell);
//         Assert.AreEqual(deck.drawPile, spells);
//     }

//     [Test]
//     public void CanDrawCorrectSpell()
//     {
//         var (spell, _) = deck.Draw();
//         spell = spells.First();
//     }

//     [Test]
//     public void CanShuffle()
//     {
//         for (int i = 0; i < deck.spells.Count; i++)
//         {
//             var (spell, _) = deck.Draw();
//             if (spell != null) deck.Use(spell);
//         }
//         deck.Shuffle();
//         Assert.AreEqual(0, deck.discardPile.Count);
//         Assert.AreEqual(spells.Count, deck.drawPile.Count);
//     }

//     [Test]
//     public void CheckOnDraw()
//     {
//         SpellSlot? slot = null;
//         Spell? spell = null;
//         deck.onDraw = (SpellSlot sl, Spell sp) =>
//         {
//             slot = sl;
//             spell = sp;
//         };
//         deck.Draw();
//         Assert.AreEqual(slot, SpellSlot.Spell1);
//         Assert.AreEqual(spell, spells.First());
//     }

//     [Test]
//     public void CheckOnAdd()
//     {
//         Spell? spell = null;
//         deck.onAdd = (Deck deck, Spell sl) =>
//         {
//             spell = sl;
//         };
//         var newSpell = new Explosion();
//         deck.Add(newSpell);
//         Assert.AreEqual(newSpell, spell);
//         spells.Add(newSpell);
//         Assert.AreEqual(spells, deck.drawPile);
//     }

//     [Test]
//     public void CheckOnShuffle()
//     {
//         var isCalled = false;
//         deck.onShuffle = (Deck deck) =>
//         {
//             isCalled = true;
//         };
//         deck.Shuffle();
//         Assert.True(isCalled);
//     }

//     [Test]
//     public void CheckAddAfterDraw()
//     {
//         var newSpell = new Ignis();
//         deck.Add(newSpell);
//         spells.Add(newSpell);
//         var (drawed, _) = deck.Draw();
//         Assert.AreEqual(spells.First(), drawed);
//         Assert.AreEqual(spells.Skip(1), deck.drawPile);
//     }

//     [Test]
//     public void CheckAddWhileDraw()
//     {
//         var newSpell = new Ignis();

//         deck.Draw();
//         deck.Add(newSpell);
//         deck.Draw();
//         deck.Draw();

//         spells.Add(newSpell);
//         Assert.AreEqual(spells.Take(3), deck.discardPile);
//         Assert.AreEqual(spells.Skip(3).ToList().Count, deck.drawPile.Count);
//     }

//     [Test]
//     public void CheckCorrectlyUseSpell()
//     {
//         var (spell, slot) = deck.Draw();
//         Assert.AreNotEqual(null, slot);

//         // 上のassertがあるのでnullはありえない。コンパイラの警告を抑制するためにnullチェックしている。
//         if (slot is SpellSlot nonNullSlot)
//         {
//             Assert.AreEqual(spell, deck.GetSpell(nonNullSlot));
//             if (spell != null) deck.Use(spell);
//             Assert.AreEqual(null, deck.GetSpell(nonNullSlot));
//         }
//     }
// }
