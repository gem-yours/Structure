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


    // 山札・捨札はインデックスが小さいものがデッキトップ
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
    public bool isShuffling { get; private set; } = false;

    public delegate void OnAdd(Deck deck, Spell spell);
    public delegate void OnDraw(SpellSlot slot, Spell spell);
    public delegate void OnProgress(Spell spell, float progress, bool isActive);
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
    public OnProgress? onProgress { set; private get; } = null;
    public OnShuffle? onShuffle { set; private get; } = null;
    public OnRemove? onRemove { set; private get; } = null;


    private float shuffleTime = 0;

    public Deck(List<Spell> spells, float shuffleTime)
    {
        this._spells = new List<Spell>(spells);
        _drawPile = new List<Spell>(spells);
        _discardPile = new List<Spell>();
        this.shuffleTime = shuffleTime;
    }

    public bool canDraw
    {
        get
        {
            return drawPile.Count > 0 && slots.GetEmptySlot() != null;
        }
    }

    public bool needShuffle
    {
        get
        {
            return drawPile.Count == 0 && slots.numberOfEquipments == 0;
        }
    }

    public Spell? GetSpell(SpellSlot slot)
    {
        return slots.GetSpell(slot);
    }

    public void Add(Spell spell)
    {
        _spells.Add(spell);
        if (!isShuffling)
        {
            _drawPile.Add(spell);
        }
        else
        {
            _discardPile.Add(spell);
        }

        foreach (OnAdd listener in onAdds)
        {
            listener(this, spell);
        }
    }

    // 山札の上からnumberOfCandiates個のスペルを返す
    public List<Spell?> LatestCandidates(int numberOfCandidates)
    {
        return new List<Spell?>(_drawPile.Take(numberOfCandidates).ToList());
    }

    public IEnumerator Draw()
    {
        if (!canDraw) yield break;

        var spell = _drawPile.First();
        var slot = slots.Equip(spell, false);

        if (slot is null)
        {
            yield break;
        }
        _discardPile.Add(spell);
        _drawPile.Remove(spell);
        if (onDraw is not null) onDraw((SpellSlot)slot, spell);

        yield return AnimationUtil.Linear(spell.drawTime, (current) =>
        {
            if (onProgress is not null) onProgress(spell, current, current == spell.drawTime);
        });
        slots.Activete((SpellSlot)slot, true);
    }

    public SpellSlot? GetSpellSlot(Spell spell)
    {
        return slots.GetSpellSlot(spell);
    }

    private IEnumerator Shuffle()
    {
        isShuffling = true;
        yield return new WaitForSeconds(shuffleTime);
        _discardPile.RemoveAll(x => true);
        _drawPile = _spells.OrderBy(x => Guid.NewGuid()).ToList();
        if (onShuffle != null) onShuffle(this);
        isShuffling = false;
        yield return null;
    }

    public void Use(Spell spell)
    {
        var slot = slots.RemoveSpell(spell);
        if (slot == null) return;
        if (onRemove != null) onRemove((SpellSlot)slot);
    }


    public void ContinuouslyDraw(MonoBehaviour monobehaviour)
    {
        monobehaviour.StartCoroutine(_ContinuouslyDraw());
    }

    private IEnumerator _ContinuouslyDraw()
    {
        // すぐにドローしてしまうとUIが崩れる
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            if (needShuffle)
            {
                yield return Shuffle();
            }

            if (!canDraw)
            {
                yield return null;
                continue;
            }

            yield return Draw();
            yield return null;
        }
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
    public Dictionary<SpellSlot, Spell?> currentSpells { get; private set; } = new Dictionary<SpellSlot, Spell?>();
    public Dictionary<SpellSlot, bool> areActive { get; private set; } = new Dictionary<SpellSlot, bool>();

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

        areActive.Add(SpellSlot.Spell1, false);
        areActive.Add(SpellSlot.Spell2, false);
        areActive.Add(SpellSlot.Spell3, false);

    }

    public SpellSlot? GetEmptySlot()
    {
        if (currentSpells[SpellSlot.Spell1] == null) return SpellSlot.Spell1;
        if (currentSpells[SpellSlot.Spell2] == null) return SpellSlot.Spell2;

        if (currentSpells[SpellSlot.Spell3] == null) return SpellSlot.Spell3;
        return null;
    }

    public SpellSlot? Equip(Spell spell, bool isActive = true)
    {
        var slot = GetEmptySlot();
        if (slot == null) return null;
        currentSpells[(SpellSlot)slot] = spell;
        areActive[(SpellSlot)slot] = isActive;
        return slot;
    }

    public void Activete(SpellSlot slot, bool active)
    {
        areActive[slot] = active;
    }

    public Spell? GetSpell(SpellSlot slot)
    {
        return currentSpells[slot];
    }

    public SpellSlot? GetSpellSlot(Spell spell)
    {
        return currentSpells.Keys.ToArray()
        .Select(x => (SpellSlot?)x)
        .DefaultIfEmpty(null)
        .FirstOrDefault(key => key is not null && currentSpells[(SpellSlot)key] == spell);
    }

    public SpellSlot? RemoveSpell(Spell spell)
    {
        var key = GetSpellSlot(spell);
        if (key != null)
        {
            currentSpells[(SpellSlot)key] = null;
        }
        return key;
    }
}