using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Spell
{
    Sprite image { get; }
    string description { get; }
    float damage { get; }
}
