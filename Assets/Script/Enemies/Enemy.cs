using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Enemy
{
    float hp { get; }
    int exp { get; }
    void OnHit(Spell spell);
}
