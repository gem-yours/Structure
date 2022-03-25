using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour, Spell
{
    public Sprite image { get; } = Resources.Load("SpellIcon/FireBolt") as Sprite;
    public string description { get; } = "一番近い敵に向かって爆発する火球を発射する。";
    public float damage { get; } = 10;
}
