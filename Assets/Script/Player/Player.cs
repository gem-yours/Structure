using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#nullable enable
public class Player : MonoBehaviour, Living
{
    public GameObject? directionIndicator;
    public ExpManager expManager { private set; get; } = new ExpManager();
    public float speed { private set; get; } = 0.15f;

    public Deck deck = new Deck(
        new List<Spell> { new Ignis(), new Ignis(), new Ignis(), new Ignis(), new Explosion() }
        );

#pragma warning disable CS8618
    private DrawManager drawManager;
#pragma warning restore CS8618

    public delegate GameObject? NearestEnemy(Vector2 location);
    public NearestEnemy nearestEnemy = (Vector2 location) => { return null; };

    public Living.DamageAnimation? damageAnimation { set; private get; }
    public Living.DeadAnimation? deadAnimation { set; private get; }

    private float drawTime = 0.25f;
    private float shuffleTime = 2f;

    private bool isInvincible = false;
    private float invincibleDuration = 1f;

    private Vector2 movingDirection = Vector2.zero;
    private Rigidbody2D? rb2D;
    private Animator? animator;

    private float draggingThreshold = 50;

    public void ChangeMoveDirection(Vector2 direction)
    {
        movingDirection = direction;

        animator?.SetBool("isWalking", direction != Vector2.zero);
    }

    private void Move()
    {
        if (rb2D == null) return;
        var current = rb2D.position;
        rb2D.MovePosition(Vector2.MoveTowards(current, current + movingDirection, speed));

        var isLeft = (movingDirection.x < 0) ? 1 : -1;
        if (isLeft != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x) * isLeft,
                transform.localScale.y,
                transform.localScale.z);
        }
    }

    public void IndicateDirection(Vector2 direction)
    {
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
        var spell = new Ignis();
        var boltObject = Instantiate(spell.prefab, transform.position, transform.rotation) as GameObject;
        if (boltObject == null) return;
        var bolt = boltObject.GetComponent<BoltProjectile?>();
        if (bolt == null) return;
        bolt.spell = new Ignis();
        bolt.Target(nearestEnemy(transform.position));
    }

    public void Cast(SpellSlot slot)
    {
        IndicateDirection(Vector2.zero);
        var spell = deck.GetSpell(slot);
        if (spell == null) return;

        StartCoroutine(Casting(spell));
    }

    private IEnumerator Casting(Spell spell)
    {
        for (int time = 0; time < spell.magazine; time++)
        {
            // インジケータに合わせて発射位置をずらす
            var spellEffect = (Instantiate(spell.prefab, transform.position - new Vector3(0, 0.5f, 0), Quaternion.identity) as GameObject)?.GetComponent<SpellEffect?>();
            if (spellEffect == null) break;
            spellEffect.spell = spell;
            spellEffect.Target(nearestEnemy(transform.position));
            // 最後の一発はディレイを入れる必要がない
            if (time != spell.magazine - 1)
            {
                yield return new WaitForSeconds(spell.delay);
            }
        }
        deck.Use(spell);
    }

    private IEnumerator DrawSpell()
    {
        while (true)
        {
            yield return drawManager.Draw();
        }
    }


    public IEnumerator OnHit(float damage)
    {
        isInvincible = true;
        animator?.SetTrigger("damaged");
        // TODO: Coroutine内で別のCoroutineを起動するのは良いやり方なのか？
        if (damageAnimation != null) StartCoroutine(damageAnimation());
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (isInvincible) return;
            var enemy = collision.gameObject.GetComponent<Enemy>();
            StartCoroutine(OnHit(enemy.damage));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        drawManager = new DrawManager(deck, drawTime, shuffleTime);

        deck.onAdd = (Deck deck, Spell spell) =>
        {
            StartCoroutine(DrawSpell());
        };
        StartCoroutine(DrawSpell());
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;
        if (movingDirection != Vector2.zero)
        {
            Move();
        }
    }
}

public class DrawManager
{
    Deck deck;

    float drawTime;
    float shuffleTime;

    public DrawManager(Deck deck, float drawTime, float shuffleTime)
    {
        this.deck = deck;
        this.drawTime = drawTime;
        this.shuffleTime = shuffleTime;
    }

    public IEnumerator Draw()
    {

        if (deck.needShuffle)
        {
            yield return new WaitForSeconds(shuffleTime);
            deck.Shuffle();
        }

        if (!deck.canDraw)
        {
            yield break;
        }

        yield return new WaitForSeconds(drawTime);
        deck.Draw();
    }
}
