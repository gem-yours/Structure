using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBolt : Spell
{
    public Sprite image { get; } = null;
    public string description { get; } = "一番近い敵に向かって火の玉を発射する";
    public float damage { get; } = 1;

}
