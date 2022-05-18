using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;

public class BoltProjectile : MonoBehaviour, SpellEffect
{
    public Spell spell { set; protected get; }
    public Vector2 direction = Vector2.right;

    private Rigidbody2D rb2D;
    private Coroutine fadingCoroutine;

    public void Target(Vector2 target)
    {
        direction = ((Vector3)target - transform.position).normalized;
        if (direction.magnitude == 0)
        {
            direction = Vector2.right;
            return;
        }
        transform.rotation = Quaternion.FromToRotation(Vector2.right, direction);
        // ターゲットに向けて少しずらさないと意図せず壁にぶつかることがある
        transform.position += (Vector3)direction;
    }

    public void Move()
    {
        var current = rb2D.position;
        rb2D.MovePosition(Vector2.MoveTowards(current, current + direction * spell.speed * Time.deltaTime, spell.speed * Time.deltaTime));

        rb2D.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
    }

    protected IEnumerator Fade()
    {
        yield return new WaitForSeconds(spell.lifetime);
        Destroy(gameObject);
    }

    protected virtual void OnHit()
    {
        Destroy(gameObject);
    }

    protected void OnTouchedOther(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            StopCoroutine(fadingCoroutine);
            var enemy = other.gameObject.GetComponent<Enemy>();
            enemy.OnHit(spell.damage);
            OnHit();
        }
        if (other.gameObject.tag == "Wall")
        {
            StopCoroutine(fadingCoroutine);
            var wall = other.gameObject.GetComponent<Wall>();
            wall.OnHit(spell.damage);
            OnHit();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        OnTouchedOther(other);
    }

    protected virtual void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        fadingCoroutine = StartCoroutine(Fade());
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }
}