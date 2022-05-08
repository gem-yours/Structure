using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ignis : Spell
{
    override public string identifier { get; } = "Ignis";
    override public string name { get; } = "イグニス";
    override public string description { get; } = "一番近い敵に向かって火の玉を発射する";
    override public float damage { get; } = 1;
    override public int magazine { get; } = 1;
    override public float delay { get; } = 1f;
    public override float duration { get; } = 0f;
    override public float speed { get; } = 0.5f;
    override public float range { get; } = 1f;
    override protected string audioPath { get; } = "ignition";
}
