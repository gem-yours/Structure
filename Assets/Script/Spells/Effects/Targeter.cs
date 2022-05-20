using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public interface ITargeter
{
    Vector2 SearchTarget(Spell spell);
}
