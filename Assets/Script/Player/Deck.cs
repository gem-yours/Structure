using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Deck
{
    public List<Spell> spells { private set; get; }

    public List<Spell> remaingSpells { private set; get; }

    public Deck(List<Spell> spells)
    {
        this.spells = new List<Spell>(spells);
        remaingSpells = new List<Spell>(spells);
    }

    public void AddSpell(Spell spell)
    {
        spells.Add(spell);
        remaingSpells.Add(spell);
    }

    public Spell PickSpell()
    {
        if (remaingSpells.Count == 0)
        {
            Shuffle();
        }
        return remaingSpells.Last();
    }

    private void Shuffle()
    {
        remaingSpells = spells.OrderBy(x => Guid.NewGuid()).ToList();
    }
}
