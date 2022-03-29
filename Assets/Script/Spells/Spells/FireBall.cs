using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Spell
{
    override public string name { get; } = "FireBall";
    override public string description { get; } = "一番近い敵に向かって爆発する火球を発射する。";
    override public float damage { get; } = 10;
    override public int magazine { get; } = 1;
    override public float delay { get; } = 1f;
}
