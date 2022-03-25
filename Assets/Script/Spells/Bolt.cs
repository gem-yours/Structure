using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bolt : MonoBehaviour, Spell
{
    public Vector2 direction = Vector2.right;
    public float speed = 0.5f;

    abstract public Sprite image { get; }
    abstract public string description { get; }
    abstract public float damage { get; }
    private Rigidbody2D rb2D;

    public void Target(GameObject target)
    {
        if (target == null || target.transform == null)
        {
            direction = Vector2.right;
            return;
        }
        direction = (target.transform.position - transform.position).normalized;
        if (direction.magnitude == 0)
        {
            direction = Vector2.right;
            return;
        }
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void Move()
    {
        var current = rb2D.position;
        rb2D.MovePosition(Vector2.MoveTowards(current, current + direction, speed));


        rb2D.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
    }

    protected virtual IEnumerator Fade()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            var enemy = other.gameObject.GetComponent<Enemy>();
            enemy.OnHit(this);
            Destroy(gameObject);
        }
        if (other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Move();
    }
}