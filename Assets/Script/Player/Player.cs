using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#nullable enable
public class Player : MonoBehaviour, Living, ITargeter
{
    public GameObject? directionIndicator;
    public ExpManager expManager { private set; get; } = new ExpManager();
    public static float speed { private set; get; } = 10f; // 2.5f
    public Deck deck = new Deck(
                new List<Spell> { new Explosion(), new Ignis(), new Ignis(), new Ignis(), new Ignis() },
                2f
                );

#pragma warning disable CS8618
    private AudioSource audioSource;
    private Rigidbody2D rb2D;
    private Animator animator;
#pragma warning restore CS8618

    public delegate GameObject? NearestEnemy(Vector2 location);
    public NearestEnemy nearestEnemy = (Vector2 location) => { return null; };

    public Living.DamageAnimation? damageAnimation { set; private get; }
    public Living.DeadAnimation? deadAnimation { set; private get; }

    private bool isAttacking = false;

    private bool isInvincible = false;
    private float invincibleDuration = 1f;

    private Vector2 movingDirection = Vector2.zero;

    private float draggingThreshold = 50;
    private Vector2 indicatorDirection = Vector2.zero;

    public void ChangeMoveDirection(Vector2 direction)
    {
        movingDirection = direction;

        animator?.SetBool("isWalking", direction != Vector2.zero);
    }

    private void Move()
    {
        if (rb2D == null) return;
        var current = rb2D.position;
        rb2D.MovePosition(Vector2.MoveTowards(current, current + movingDirection, speed * Time.deltaTime));

        // 攻撃中は向きを変えない
        if (isAttacking) return;
        ChangeFacingDirection(movingDirection);
    }

    private void ChangeFacingDirection(Vector2 direction)
    {
        if (direction.magnitude == 0) return;
        var isLeft = (direction.x < 0) ? 1 : -1;
        if (isLeft != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x) * isLeft,
                transform.localScale.y,
                transform.localScale.z);
        }
    }

    public void Dragged(SpellSlot slot, Vector2 direction)
    {
        var spell = deck.GetSpell(slot);
        if (spell is null) return;
        if (spell.targetType == Spell.TargetType.Direction)
        {
            IndicateDirection(direction);
        }
    }

    private void IndicateDirection(Vector2 direction)
    {
        indicatorDirection = direction;

        if (directionIndicator == null) return;

        if (direction.magnitude < draggingThreshold)
        {
            directionIndicator.SetActive(false);
            return;
        }
        directionIndicator.SetActive(true);
        directionIndicator.transform.rotation = Quaternion.FromToRotation(Vector2.up, direction);
        directionIndicator.transform.localScale = new Vector3(
            0.1f,
            (0.1f - Mathf.Abs(direction.normalized.y) * 0.1f) / 2 + 0.05f,
            0.1f
        );
    }

    public void Attack()
    {
        if (isAttacking) return;
        StartCoroutine(_Attack());
    }

    private IEnumerator _Attack()
    {
        isAttacking = true;
        animator?.SetTrigger("attack");
        var enemy = nearestEnemy(transform.position);
        if (enemy != null) ChangeFacingDirection(enemy.transform.position - transform.position);
        var rawSpeed = speed;
        speed *= 0.75f;

        yield return new WaitForSeconds(0.75f);

        var spell = new Ignis();
        var boltObject = Instantiate(spell.prefab, transform.position, transform.rotation) as GameObject;
        if (boltObject == null)
        {
            isAttacking = false;
            yield break;
        }
        var bolt = boltObject.GetComponent<BoltProjectile?>();
        if (bolt == null)
        {
            isAttacking = false;
            speed = rawSpeed;
            yield break;
        }
        bolt.spell = new Ignis();

        bolt.Target(this);
        speed = rawSpeed;

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        speed = rawSpeed;
    }

    public void Cast(SpellSlot slot)
    {
        if (indicatorDirection.magnitude < draggingThreshold)
        {
            IndicateDirection(Vector2.zero);
            return;
        }
        var spell = deck.GetSpell(slot);
        if (spell == null)
        {
            IndicateDirection(Vector2.zero);
            return;
        }

        audioSource.clip = spell.audioClip;
        audioSource.Play();

        StartCoroutine(Casting(spell, indicatorDirection));
        IndicateDirection(Vector2.zero);
    }

    private IEnumerator Casting(Spell spell, Vector2 direction)
    {
        for (int time = 0; time < spell.magazine; time++)
        {
            // インジケータに合わせて発射位置をずらす
            var spellEffect = (Instantiate(spell.prefab, transform.position - new Vector3(0, 0.5f, 0), Quaternion.identity) as GameObject)?.GetComponent<SpellEffect?>();
            if (spellEffect == null) break;
            spellEffect.spell = spell;
            spellEffect.Target(this);
            // 最後の一発はディレイを入れる必要がない
            if (time != spell.magazine - 1)
            {
                yield return new WaitForSeconds(spell.delay);
            }
        }
        deck.Use(spell);
    }

    public Vector2 SearchTarget(Spell spell)
    {
        switch (spell.targetType)
        {
            case Spell.TargetType.Auto:
                var enemy = nearestEnemy(transform.position);
                if (enemy is null) return Vector2.left * transform.localScale.x;
                return enemy.transform.position;
            case Spell.TargetType.Direction:
                return indicatorDirection;
        }
        return Vector2.zero;
    }

    public IEnumerator OnHit(float damage)
    {
        isInvincible = true;
        if (!isAttacking)
        {
            animator?.SetTrigger("damaged");
        }
        // TODO: Coroutine内で別のCoroutineを起動するのは良いやり方なのか？
        if (damageAnimation != null) StartCoroutine(damageAnimation());
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    private void OnHit(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (isInvincible) return;
            var enemy = collision.gameObject.GetComponent<Enemy>();
            StartCoroutine(OnHit(enemy.damage));
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        OnHit(other);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        OnHit(other);
    }

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        deck.ContinuouslyDraw(this);
        deck.onAdd = (Deck deck, Spell spell) =>
        {
        };
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.identity;
        if (movingDirection != Vector2.zero)
        {
            Move();
        }
    }
}