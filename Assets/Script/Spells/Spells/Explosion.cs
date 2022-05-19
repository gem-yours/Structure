using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Spell
{
    override public string identifier { get; } = "Explosion";
    override public string name { get; } = "エクスプロージョン";
    override public string description { get; } = "一番近い敵に向かって爆発する火球を発射する。";
    override public float damage { get; } = 10;
    override public int magazine { get; } = 1;
    override public float delay { get; } = 1f;
    override public float duration { get; } = 1f;
    override public float speed { get; } = 5f;
    override public float range { get; } = 5f;
    override public TargetType targetType { get; } = TargetType.Direction;
    override public float drawTime { get; } = 1f;

    override protected string audioPath { get; } = "ignition";
}
