using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : Bolt
{
    public override float damage
    {
        get { return 10; }
    }
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
