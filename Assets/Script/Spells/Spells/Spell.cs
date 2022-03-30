using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell
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

    public GameObject prefab
    {
        get
        {
            return Resources.Load<GameObject>("Effects/" + name);
        }
    }

}