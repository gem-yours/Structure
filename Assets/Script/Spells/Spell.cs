using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Spell
{
    string name { get; }
    string description { get; }
    float damage { get; }
    int magazine { get; }
    float delay { get; }
}

public class SpellUtil
{
    public static string getImageName(Spell spell)
    {
        return "SpellIcon/" + spell.name;
    }
}
