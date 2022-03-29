using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBolt : Spell
{
    override public string name { get; } = "FireBolt";
    override public string description { get; } = "一番近い敵に向かって火の玉を発射する";
    override public float damage { get; } = 1;
    override public int magazine { get; } = 2;
    override public float delay { get; } = 1f;
}
