using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public abstract class Spell : IEquatable<Spell>
{
    public abstract string name { get; }
    public abstract string description { get; }
    public abstract float damage { get; }
    public abstract int magazine { get; }
    public abstract float delay { get; }

    public Sprite image
    {
        get
        {
            return Resources.Load<Sprite>("SpellIcon/" + name);
        }
    }

    public UnityEngine.Object prefab
    {
        get
        {
            return Resources.Load<UnityEngine.Object>("Effects/" + name);
        }
    }

    public bool Equals(Spell? other)
    {
        return this == other;
    }
}