using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Enemy
{
    float hp { get; }
    void OnHit(Spell spell);
}
