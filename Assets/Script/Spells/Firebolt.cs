using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBolt : Spell
{
    public string name { get; } = "FireBolt";
    public string description { get; } = "一番近い敵に向かって火の玉を発射する";
    public float damage { get; } = 1;
    public int magazine { get; } = 2;
    public float delay { get; } = 1f;
}
