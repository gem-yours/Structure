using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeProjection : MonoBehaviour, SpellEffect
{
    public Spell spell { set; private get; }
    public Vector2 direction = Vector2.right;
    public float speed { set; private get; } = 0.1f;

    private bool isExploded = false;

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
        transform.rotation = Quaternion.FromToRotation(Vector2.right, direction);
    }

    public void Move()
    {
        if (isExploded)
        {
            return;
        }
        var current = rb2D.position;
        rb2D.MovePosition(Vector2.MoveTowards(current, current + direction, speed));


        rb2D.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
    }

    private IEnumerator Explode()
    {
        isExploded = true;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // TOOO: switchとかで書き直す
        if (other.gameObject.tag == "Enemy")
        {
            var enemy = other.gameObject.GetComponent<Enemy>();
            enemy.OnHit(spell);
            if (isExploded == false)
            {
                StartCoroutine(Explode());
            }
        }
        if (other.gameObject.tag == "Wall")
        {
            if (isExploded == false)
            {
                StartCoroutine(Explode());
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
}
