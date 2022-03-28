using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Spell
{
    public string name { get; } = "FireBall";
    public string description { get; } = "一番近い敵に向かって爆発する火球を発射する。";
    public float damage { get; } = 10;
    public int magazine { get; } = 1;
    public float delay { get; } = 1f;
}
