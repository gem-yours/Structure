using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElement : MonoBehaviour, Enemy, Living
{
    public float speed { get; private set; } = 1f;
    public float hp { get; private set; } = 2;
    public float damage { get; private set; } = 10;
    public int exp { get; private set; } = 1;

    public GameObject target { set; private get; }
    public Enemy.OnDead onDead { set; private get; } = (Enemy enemy) => { };

    public Living.DamageAnimation damageAnimation { set; private get; }

    public Living.DeadAnimation deadAnimation { set; private get; }

    protected Rigidbody2D rb2D;

    private bool isDead = false;

    public void OnHit(float damage)
    {
        if (damage <= 0) return;
        hp -= damage;
        StartCoroutine(damageAnimation());
        checkHp();
    }

    private void MoveToPlayer()
    {
        Vector2 direction = (target.transform.position - rb2D.transform.position).normalized;
        Vector2 current = rb2D.transform.position;
        rb2D.MovePosition(Vector2.MoveTowards(current, current + direction, speed * Time.deltaTime));

        // 回転・反転
        var isLeft = (direction.x > 0) ? 1 : -1;
        if (isLeft != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x) * isLeft,
                transform.localScale.y,
                transform.localScale.z);
        }
        rb2D.transform.rotation = Quaternion.FromToRotation(isLeft == -1 ? Vector3.left : Vector3.right, direction);
    }

    private void checkHp()
    {
        if (hp > 0 || isDead) return;
        Destroy(rb2D);
        isDead = true;
        StartCoroutine(dead());
    }

    private IEnumerator dead()
    {
        yield return deadAnimation();
        onDead(this);
    }

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        MoveToPlayer();
    }
}
