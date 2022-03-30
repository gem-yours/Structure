using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Deck
{
    private List<Spell> _spells = new List<Spell>();
    public List<Spell> spells
    {
        get
        {
            return new List<Spell>(_spells);
        }
    }

    private List<Spell> _remaingSpells = new List<Spell>();
    public List<Spell> remaingSpells
    {
        get
        {
            return new List<Spell>(_remaingSpells);
        }
    }

    private List<Spell> _discardedSpells = new List<Spell>();
    public List<Spell> discardedSpells
    {
        get
        {
            return new List<Spell>(_discardedSpells);
        }
    }

    public delegate void OnChange(Deck deck);
    public delegate void OnDraw(Deck deck, Spell spell);

    public OnChange onAdd { set; private get; } = (Deck deck) => { };
    public OnDraw onDraw { set; private get; } = (Deck deck, Spell spell) => { };
    public OnChange onShuffle { set; private get; } = (Deck deck) => { };


    public Deck() : this(new List<Spell>())
    {

    }
    public Deck(List<Spell> spells)
    {
        this._spells = new List<Spell>(spells);
        _remaingSpells = new List<Spell>(spells);
        _discardedSpells = new List<Spell>();
    }

    private void _AddSpell(Spell spell)
    {
        _spells.Add(spell);
        _remaingSpells.Add(spell);
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
        if (_remaingSpells.Count <= numberOfCandidates)
        {
            return _remaingSpells;
        }

        var candidates = _remaingSpells.Skip(_remaingSpells.Count - numberOfCandidates).ToList();
        while (candidates.Count < numberOfCandidates)
        {
            candidates.Add(null);
        }
        return new List<Spell>(candidates);
    }

    public Spell DrawSpell()
    {
        if (_remaingSpells.Count == 0)
        {
            Shuffle();
        }
        var spell = _remaingSpells[0];
        _discardedSpells.Add(spell);
        _remaingSpells.Remove(spell);
        onDraw(this, spell);
        return spell;
    }

    private void Shuffle()
    {
        _discardedSpells.RemoveAll(x => true);
        _remaingSpells = _spells.OrderBy(x => Guid.NewGuid()).ToList();
        onShuffle(this);
    }
}
