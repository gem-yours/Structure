using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#nullable enable
public class Player : MonoBehaviour, Living, ITargeter
{
    public float maxHp { private set; get; } = 100;
    public float currentHp { private set; get; } = 1;
    public ExpManager expManager { private set; get; } = new ExpManager();
    public static float speed { private set; get; } = 10f; // 2.5f
    public Deck deck =
        new Deck(
                new List<Spell> { new Explosion(), new Ignis(), new Ignis(), new Ignis(), new Ignis() },
                2f
        );
    private Dictionary<SpellSlot, bool> isCasting = new Dictionary<SpellSlot, bool> {
        { SpellSlot.Spell1, false },
        { SpellSlot.Spell2, false },
        { SpellSlot.Spell3, false },
    };
    public delegate void OnCasting(Spell spell, float current); // currentは0 ~ 1;
    public OnCasting? onCasting = null;
    public delegate void OnDamaged(float hp);

    public OnDamaged? onDamaged = null;
    public delegate void OnDead();
    public OnDead? onDead = null;

#pragma warning disable CS8618
    public Indicator indicator;
    private AudioSource audioSource;
    private Rigidbody2D rb2D;
    private Animator animator;
#pragma warning restore CS8618

    public delegate GameObject? NearestEnemy(Vector2 location);
    public NearestEnemy nearestEnemy = (Vector2 location) => { return null; };

    public Vector2 facingDirection
    {
        get
        {
            return Vector2.left * transform.localScale.x;
        }
    }

    public Living.DamageAnimation? damageAnimation { set; private get; }
    public Living.DeadAnimation? deadAnimation { set; private get; }

    private bool isAttacking = false;

    private bool isInvincible = false;
    private float invincibleDuration = 1f;

    private Vector2 movingDirection = Vector2.zero;


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
            indicator.IndicateDirection(direction);
        }
    }

    public void Pushed(SpellSlot slot)
    {
        var spell = deck.GetSpell(slot);
        if (spell is null) return;
        if (spell.targetType == Spell.TargetType.Direction)
        {
            indicator.IndicateDirection(facingDirection);
        }
    }

    public void EndDragging(SpellSlot slot)
    {
        var spell = deck.GetSpell(slot);
        if (spell is null) return;
        if (indicator.IsActive(spell.targetType))
        {
            Cast(slot);
        }
        indicator.HideIndicator();
    }

    public void Clicked(SpellSlot slot)
    {
        var spell = deck.GetSpell(slot);
        if (spell is null) return;
        if (spell.targetType == Spell.TargetType.Auto)
        {
            Cast(slot);
        }
        indicator.HideIndicator();
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
        if (isCasting[slot])
        {
            return;
        }
        var spell = deck.GetSpell(slot);
        if (spell == null)
        {
            return;
        }

        audioSource.clip = spell.audioClip;
        audioSource.Play();

        StartCoroutine(Casting(slot, spell));
    }

    private IEnumerator Casting(SpellSlot slot, Spell spell)
    {
        isCasting[slot] = true;
        for (int time = 0; time < spell.magazine; time++)
        {
            // インジケータに合わせて発射位置をずらす
            var spellEffect = (Instantiate(spell.prefab, transform.position - new Vector3(0, 0.5f, 0), Quaternion.identity) as GameObject)?.GetComponent<SpellEffect?>();
            if (spellEffect == null) break;
            spellEffect.spell = spell;
            spellEffect.Target(this);
            // 最後の一発はディレイを入れる必要がない
            yield return AnimationUtil.Linear(spell.delay, (current) =>
            {
                if (onCasting is not null)
                {
                    var progressPerCast = 1f / spell.magazine;
                    onCasting(spell, (current + time) * progressPerCast);
                }
            });
        }

        if (onCasting is not null) onCasting(spell, 0);
        isCasting[slot] = false;
        deck.Use(spell);
    }

    public Vector2 SearchTarget(Spell spell)
    {
        switch (spell.targetType)
        {
            case Spell.TargetType.Auto:
                var enemy = nearestEnemy(transform.position);
                if (enemy is null) return facingDirection;
                return enemy.transform.position - transform.position;
            case Spell.TargetType.Direction:
                return indicator.direction;
        }
        return Vector2.zero;
    }

    private void takeDamage(float damage)
    {
        if (damage <= 0) return;
        currentHp -= damage;
        if (currentHp < 0)
        {
            currentHp = 0;
            if (onDead is not null) onDead();
        }
    }

    public IEnumerator OnHit(float damage)
    {
        isInvincible = true;
        if (!isAttacking)
        {
            animator?.SetTrigger("damaged");
        }
        takeDamage(damage);
        if (onDamaged is not null) onDamaged(currentHp);
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
        if (movingDirection != Vector2.zero)
        {
            Move();
        }
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
    }
}