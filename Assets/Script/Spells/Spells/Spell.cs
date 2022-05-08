using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
// 時間単位は秒
public abstract class Spell : IEquatable<Spell>
{
    // prefabやアイコンの名前
    public abstract string identifier { get; }
    public abstract string name { get; }
    public abstract string description { get; }
    public abstract float damage { get; }
    public abstract int magazine { get; }
    public abstract float delay { get; }
    public abstract float duration { get; }
    public abstract float speed { get; }
    public abstract float range { get; }
    protected abstract string audioPath { get; }

    public float lifetime
    {
        get
        {
            return range / speed;
        }
    }

    public Sprite image
    {
        get
        {
            return Resources.Load<Sprite>("SpellIcon/" + identifier);
        }
    }

    public UnityEngine.Object prefab
    {
        get
        {
            return Resources.Load<UnityEngine.Object>("Effects/" + identifier);
        }
    }

    public AudioClip? audioClip
    {
        get
        {
            return Resources.Load<AudioClip>("Sound/" + audioPath);
        }
    }

    public bool Equals(Spell? other)
    {
        return this == other;
    }
}