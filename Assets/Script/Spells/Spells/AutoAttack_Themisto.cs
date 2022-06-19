using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttack_Themisto : Spell
{
    override public string identifier { get; } = "AA_Themisto";
    override public string name { get; } = "通常攻撃";
    override public string description { get; } = "一番近い敵に向かって火の玉を発射する";
    override public float damage { get; } = 1;
    override public int magazine { get; } = 1;
    override public float delay { get; } = 0.5f;
    public override float duration { get; } = 0f;
    override public float speed { get; } = 5f;
    override public float range { get; } = 10f;
    override public TargetType targetType { get; } = TargetType.Auto;
    override public float drawTime { get; } = 0f;

    override protected string audioPath { get; } = "";
}
