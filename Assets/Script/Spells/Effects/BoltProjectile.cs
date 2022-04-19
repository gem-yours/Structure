using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        rb2D.MovePosition(Vector2.MoveTowards(current, current + direction, spell.speed));

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
        StopCoroutine(fadingCoroutine);
        if (other.gameObject.tag == "Enemy")
        {
            var enemy = other.gameObject.GetComponent<Enemy>();
            enemy.OnHit(spell.damage);
            OnHit();
        }
        if (other.gameObject.tag == "Wall")
        {
            OnHit();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        OnTouchedOther(other);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        fadingCoroutine = StartCoroutine(Fade());
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Move();
    }
}