using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Enemy
{
    float hp { get; }
    float damage { get; }
    int exp { get; }
    Transform transform { get; }
    GameObject target { set; }
    delegate void OnDead(Enemy enemy);
    OnDead onDead { set; }
    void OnHit(float damage);
}
