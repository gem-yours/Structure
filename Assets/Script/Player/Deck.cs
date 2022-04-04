using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#nullable enable
public class Deck
{
    private EquipmentSlot slots = new EquipmentSlot();
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
    public delegate void OnDraw(SpellSlot slot, Spell spell);
    public delegate void OnShuffle(Deck deck);
    public delegate void OnRemove(SpellSlot slot);

    public OnAdd? onAdd { set; private get; } = null;
    public OnDraw? onDraw { set; private get; } = null;
    public OnShuffle? onShuffle { set; private get; } = null;
    public OnRemove? onRemove { set; private get; } = null;

    private float drawTime = 0.75f;
    private float shuffleTime = 2f;

    public Deck(List<Spell> spells, float drawTime, float shuffleTime)
    {
        this._spells = new List<Spell>(spells);
        _remaingSpells = new List<Spell>(spells);
        _discardedSpells = new List<Spell>();
    }

    public bool canDraw
    {
        get
        {
            return slots.GetEmptySlot() != null;
        }
    }

    public Spell? GetSpell(SpellSlot slot)
    {
        return slots.GetSpell(slot);
    }

    public void AddSpell(Spell spell)
    {
        _spells.Insert(0, spell);
        _remaingSpells.Add(spell);
        if (onAdd != null) onAdd(this, spell);
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

    public IEnumerator DrawSpell()
    {
        if (slots.GetEmptySlot() == null) yield break;

        if (_remaingSpells.Count == 0)
        {
            if (slots.isEmpty)
            {
                yield return Shuffle();
            }
            else
            {
                yield break;
            }
        }

        yield return new WaitForSeconds(drawTime);

        var spell = _remaingSpells.Last();
        var slot = slots.Equip(spell);
        _discardedSpells.Add(spell);
        _remaingSpells.Remove(spell);
        if (onDraw != null && slot != null) onDraw((SpellSlot)slot, spell);

    }

    public IEnumerator Shuffle()
    {
        yield return new WaitForSeconds(shuffleTime);
        _discardedSpells.RemoveAll(x => true);
        _remaingSpells = _spells.OrderBy(x => Guid.NewGuid()).ToList();
        if (onShuffle != null) onShuffle(this);
    }

    public void DiscardSpell(Spell spell)
    {
        var slot = slots.RemoveSpell(spell);
        if (slot == null) return;
        if (onRemove != null) onRemove((SpellSlot)slot);
    }
}

public enum SpellSlot
{
    Spell1,
    Spell2,
    Spell3,
}


public class EquipmentSlot
{
    public Dictionary<SpellSlot, Spell?> currentSpells = new Dictionary<SpellSlot, Spell?>();

    public bool isEmpty
    {
        get
        {
            return
            currentSpells[SpellSlot.Spell1] == null &&
            currentSpells[SpellSlot.Spell2] == null &&
            currentSpells[SpellSlot.Spell3] == null;
        }
    }

    public EquipmentSlot()
    {
        currentSpells.Add(SpellSlot.Spell1, null);
        currentSpells.Add(SpellSlot.Spell2, null);
        currentSpells.Add(SpellSlot.Spell3, null);
    }

    public SpellSlot? GetEmptySlot()
    {
        if (currentSpells[SpellSlot.Spell1] == null) return SpellSlot.Spell1;
        if (currentSpells[SpellSlot.Spell2] == null) return SpellSlot.Spell2;

        if (currentSpells[SpellSlot.Spell3] == null) return SpellSlot.Spell3;
        return null;
    }

    public SpellSlot? Equip(Spell spell)
    {
        var slot = GetEmptySlot();
        if (slot == null) return null;
        currentSpells[(SpellSlot)slot] = spell;
        return slot;
    }

    public Spell? GetSpell(SpellSlot slot)
    {
        return currentSpells[slot];
    }

    public SpellSlot? RemoveSpell(Spell spell)
    {
        var key = currentSpells.Keys.ToArray()
        .Select(x => (SpellSlot?)x)
        .DefaultIfEmpty(null)
        .FirstOrDefault(key => (key != null) ? currentSpells[(SpellSlot)key] == spell : false);
        if (key != null)
        {
            currentSpells[(SpellSlot)key] = null;
        }
        return key;
    }
}