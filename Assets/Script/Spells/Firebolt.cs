using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBolt : Bolt
{
    public override Sprite image { get; } = null;
    public override string description { get; } = "一番近い敵に向かって火の玉を発射する";
    public override float damage
    { get; } = 1;
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
    }
}
