using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#nullable enable
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


    // 山札・捨札はインデックスが大きいものがデッキトップ
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

    public delegate void OnAdd(Deck deck, Spell spell);
    public delegate void OnDraw(Deck deck, Spell spell);
    public delegate void OnShuffle(Deck deck);

    public OnAdd onAdd { set; private get; } = (Deck deck, Spell spell) => { };
    public OnDraw onDraw { set; private get; } = (Deck deck, Spell spell) => { };
    public OnShuffle onShuffle { set; private get; } = (Deck deck) => { };


    public Deck() : this(new List<Spell>())
    {

    }
    public Deck(List<Spell> spells)
    {
        this._spells = new List<Spell>(spells);
        _remaingSpells = new List<Spell>(spells);
        _discardedSpells = new List<Spell>();
        onShuffle(this);
    }

    public void AddSpell(Spell spell)
    {
        _spells.Insert(0, spell);
        _remaingSpells.Add(spell);
        onAdd(this, spell);
    }

    // 山札の上からnumberOfCandiates個のスペルを返す
    // 足りない場合はnullを返す
    public List<Spell?> LatestCandidates(int numberOfCandidates)
    {
        // 指定された個数より山札が少なければ、nullを追加して返す
        if (_remaingSpells.Count <= numberOfCandidates)
        {
            var candidates = new List<Spell?>(_remaingSpells);
            while (candidates.Count < numberOfCandidates)
            {
                candidates.Add(null);
            }
            return candidates;
        }
        return new List<Spell?>(_remaingSpells.Skip(_remaingSpells.Count - numberOfCandidates).ToList());
    }

    public Spell? DrawSpell()
    {
        if (_remaingSpells.Count == 0)
        {
            return null;
        }
        var spell = _remaingSpells.Last();
        _discardedSpells.Add(spell);
        _remaingSpells.Remove(spell);
        onDraw(this, spell);
        return spell;
    }

    public void Shuffle()
    {
        _discardedSpells.RemoveAll(x => true);
        _remaingSpells = _spells.OrderBy(x => Guid.NewGuid()).ToList();
        onShuffle(this);
    }
}
