using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Deck
{
    public List<Spell> spells { private set; get; }

    public List<Spell> remaingSpells { private set; get; }
    public List<Spell> discardedSpells { private set; get; }

    public delegate void OnChange(Deck deck);

    public OnChange onAdd { set; private get; } = (Deck deck) => { };
    public OnChange onPick { set; private get; } = (Deck deck) => { };
    public OnChange onShuffle { set; private get; } = (Deck deck) => { };


    public Deck() : this(new List<Spell>())
    {

    }
    public Deck(List<Spell> spells)
    {
        this.spells = new List<Spell>(spells);
        remaingSpells = new List<Spell>(spells);
        discardedSpells = new List<Spell>();
    }

    private void _AddSpell(Spell spell)
    {
        spells.Add(spell);
        remaingSpells.Add(spell);
    }

    public void AddSpell(Spell spell)
    {
        _AddSpell(spell);
        onAdd(this);
    }

    public void AddSpells(List<Spell> spells)
    {
        foreach (Spell spell in spells)
        {
            _AddSpell(spell);
        }
        onAdd(this);
    }

    // デッキの上からnumberOfCandiates個のスペルを返す
    // 足りない場合はnull
    public List<Spell> LatestCandidates(int numberOfCandidates)
    {
        if (remaingSpells.Count <= numberOfCandidates)
        {
            return remaingSpells;
        }

        var candidates = remaingSpells.Skip(remaingSpells.Count - numberOfCandidates).ToList();
        while (candidates.Count < numberOfCandidates)
        {
            candidates.Add(null);
        }
        return candidates;
    }

    public Spell PickSpell()
    {
        if (remaingSpells.Count == 0)
        {
            Shuffle();
        }
        var spell = remaingSpells[0];
        discardedSpells.Add(spell);
        remaingSpells.Remove(spell);
        onPick(this);
        return spell;
    }

    private void Shuffle()
    {
        discardedSpells.RemoveAll(x => true);
        remaingSpells = spells.OrderBy(x => Guid.NewGuid()).ToList();
        onShuffle(this);
    }
}
