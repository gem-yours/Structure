using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public class ExplodeProjection : BoltProjectile
{
#pragma warning disable CS8618
    public GameObject explode;
    public ExplodeEffect explodeEffect;
#pragma warning restore CS8618
    private ParticleSystem? particle;

    protected override void OnHit()
    {
        explodeEffect.Explode(spell);
        explodeEffect.onDurationEnded = () => Destroy(gameObject);
        particle?.Stop(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (explodeEffect.isExploding) return;
        OnTouchedOther(other);
    }

    protected override void Start()
    {
        base.Start();
        particle = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (explodeEffect.isExploding) return;
        base.Update();
    }
}
