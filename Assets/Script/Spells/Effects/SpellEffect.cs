using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SpellEffect
{
    Spell spell { set; }
    void Target(Vector2 target);
}
