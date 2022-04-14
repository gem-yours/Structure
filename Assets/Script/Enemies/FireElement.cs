using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElement : MonoBehaviour, Enemy, Living
{
    public float speed { get; private set; } = 0.05f;
    public float hp { get; private set; } = 1;
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
        rb2D.MovePosition(Vector2.MoveTowards(current, current + direction, speed));

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


    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        MoveToPlayer();
    }
}
