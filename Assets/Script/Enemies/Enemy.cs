using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Enemy
{
    float hp { get; }
    int exp { get; }

    GameObject target { set; }
    delegate void OnDead(Enemy enemy);
    OnDead onDead { set; }
    void OnHit(float damage);
}
