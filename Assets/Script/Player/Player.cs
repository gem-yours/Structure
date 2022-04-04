using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#nullable enable
public class Player : MonoBehaviour
{
    public GameObject? directionIndicator;
    public ExpManager expManager { private set; get; } = new ExpManager();
    public float speed { private set; get; } = 0.15f;
    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }

    public Deck deck = new Deck(
        Enumerable.Repeat(new FireBolt() as Spell, 3).ToList(),
        0.75f,
        2f
        );
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

    public void Attack(SpellSlot spellSlot)
    {
        var boltObject = Instantiate(Resources.Load("Effects/Firebolt"), transform.position, transform.rotation) as GameObject;
        if (boltObject == null) return;
        var bolt = boltObject.GetComponent<BoltProjectile?>();
        if (bolt == null) return;
        bolt.spell = new FireBolt();
        bolt.Target(EnemiesManager.instance.NearestEnemy(transform.position));
    }

    public void Cast(SpellSlot slot)
    {
        IndicateDirection(Vector2.zero);
        var spell = deck.slots.GetSpell(slot);
        if (spell == null) return;

        StartCoroutine(Casting(spell));
    }

    private IEnumerator Casting(Spell spell)
    {
        for (int time = 0; time < spell.magazine; time++)
        {
            var spellEffect = (Instantiate(spell.prefab, transform.position, Quaternion.identity) as GameObject)?.GetComponent<SpellEffect?>();
            if (spellEffect == null) break;
            spellEffect.spell = spell;
            spellEffect.Target(EnemiesManager.instance.NearestEnemy(transform.position));
            yield return new WaitForSeconds(spell.delay);
        }
    }

    private IEnumerator DrawSpell()
    {
        for (SpellSlot? slot = deck.slots.GetEmptySlot(); slot != null;)
        {
            yield return deck.DrawSpell();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
