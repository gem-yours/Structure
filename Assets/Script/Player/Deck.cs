using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#nullable enable
public class Deck
{
    private EquipmentSlot slots = new EquipmentSlot();

    public int numberOfEquipments
    {
        get
        {
            return slots.numberOfEquipments;
        }
    }
    private List<Spell> _spells = new List<Spell>();
    public List<Spell> spells
    {
        get
        {
            return new List<Spell>(_spells);
        }
    }


    // 山札・捨札はインデックスが大きいものがデッキトップ
    private List<Spell> _drawPile = new List<Spell>();
    public List<Spell> drawPile
    {
        get
        {
            return new List<Spell>(_drawPile);
        }
    }

    private List<Spell> _discardPile = new List<Spell>();
    public List<Spell> discardPile
    {
        get
        {
            return new List<Spell>(_discardPile);
        }
    }

    public delegate void OnAdd(Deck deck, Spell spell);
    public delegate void OnDraw(SpellSlot slot, Spell spell);
    public delegate void OnShuffle(Deck deck);
    public delegate void OnRemove(SpellSlot slot);


    private List<OnAdd> onAdds = new List<OnAdd>();
    public OnAdd onAdd
    {
        set
        {
            onAdds.Add(value);
        }
    }
    public OnDraw? onDraw { set; private get; } = null;
    public OnShuffle? onShuffle { set; private get; } = null;
    public OnRemove? onRemove { set; private get; } = null;

    private bool isShuffling = false;

    private float drawTime = 0.75f;
    private float shuffleTime = 2f;

    public Deck(List<Spell> spells, float drawTime, float shuffleTime)
    {
        this._spells = new List<Spell>(spells);
        _drawPile = new List<Spell>(spells);
        _discardPile = new List<Spell>();
    }

    public bool canDraw
    {
        get
        {
            return slots.GetEmptySlot() != null && !isShuffling;
        }
    }

    public Spell? GetSpell(SpellSlot slot)
    {
        return slots.GetSpell(slot);
    }

    public void AddSpell(Spell spell)
    {
        _spells.Add(spell);
        // if (isShuffling)
        // {
        // TODO: 山札にスペルを追加する 
        _discardPile.Add(spell);
        return;
        // }
        // _remaingSpells.Insert(0, spell);
        // foreach (OnAdd listener in onAdds)
        // {
        //     listener(this, spell);
        // }
    }

    // 山札の上からnumberOfCandiates個のスペルを返す
    public List<Spell?> LatestCandidates(int numberOfCandidates)
    {
        return new List<Spell?>(_drawPile.Skip(_drawPile.Count - numberOfCandidates).ToList());
    }

    public IEnumerator DrawSpell()
    {
        if (!canDraw) yield break;

        if (_drawPile.Count == 0)
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

        // ドローを待っている間に山札が尽きている可能性がある
        if (_drawPile.Count == 0) yield break;

        var spell = _drawPile.Last();
        var slot = slots.Equip(spell);
        _discardPile.Add(spell);
        _drawPile.Remove(spell);
        if (onDraw != null && slot != null) onDraw((SpellSlot)slot, spell);

    }

    public IEnumerator Shuffle()
    {
        isShuffling = true;
        yield return new WaitForSeconds(shuffleTime);
        _discardPile.RemoveAll(x => true);
        _drawPile = _spells.OrderBy(x => Guid.NewGuid()).ToList();
        if (onShuffle != null) onShuffle(this);
        isShuffling = false;
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

    public int numberOfEquipments
    {
        get
        {
            return
            ((currentSpells[SpellSlot.Spell1] == null) ? 0 : 1) +
            ((currentSpells[SpellSlot.Spell2] == null) ? 0 : 1) +
            ((currentSpells[SpellSlot.Spell3] == null) ? 0 : 1);
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