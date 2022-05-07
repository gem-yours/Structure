using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stage;

#nullable enable
public class ExplodeEffect : MonoBehaviour
{
    public delegate void OnDurationEnded();
    public OnDurationEnded? onDurationEnded { set; private get; }
    public bool isExploding = false;
    private Spell? spell;
    private ParticleSystem? particle;
    private CircleCollider2D? circleCollider2D;
    private float maxRadius = 1f;

    public void Explode(Spell spell)
    {
        this.spell = spell;
        particle?.Play();
        isExploding = true;
        StartCoroutine(Exploding(spell.duration));
    }

    private IEnumerator Exploding(float duration)
    {
        // 当たり判定をだんだん大きくする
        var expandingDuration = Mathf.Max(0.5f, duration);
        var easeInOut = AnimationCurve.EaseInOut(0, 0, expandingDuration, maxRadius);
        easeInOut.AddKey(expandingDuration * 0.25f, maxRadius * 0.9f);
        for (float current = 0; current < expandingDuration; current += Time.deltaTime)
        {
            if (circleCollider2D != null) circleCollider2D.radius = easeInOut.Evaluate(current);
            yield return null;
        }

        yield return new WaitForSeconds(duration - expandingDuration);
        if (onDurationEnded != null) onDurationEnded();
    }

    private void OnHit(Collider2D other)
    {
        if (spell == null) return;
        if (other.gameObject.tag == "Enemy")
        {
            var enemy = other.gameObject.GetComponent<Enemy>();
            enemy.OnHit(spell.damage);
        }
        if (other.gameObject.tag == "Wall")
        {
            var wall = other.gameObject.GetComponent<Wall>();
            wall.OnHit(spell.damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnHit(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        OnHit(other);
    }

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particle?.Stop();
        circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.radius = 0;
    }
}
