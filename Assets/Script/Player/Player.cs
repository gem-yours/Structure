using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#nullable enable
public enum SpellSlot
{
    Spell1,
    Spell2,
    Spell3,
}
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

    public EquipmentSlot slots = new EquipmentSlot();
    public Deck deck = new Deck(
        Enumerable.Repeat(new FireBolt() as Spell, 3).ToList()
        );
    public float drawTime { get; } = 0.5f;
    public float shuffleTime { get; } = 1f;

    private Vector2 movingDirection = Vector2.zero;
    private Rigidbody2D? rb2D;
    private Animator? animator;

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

        if (direction.magnitude < 50)
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
        for (SpellSlot? slot = slots.GetEmptySlot(); slot != null; slot = slots.GetEmptySlot())
        {
            yield return new WaitForSeconds(drawTime);
            var spell = deck.DrawSpell();
            while (spell == null)
            {
                yield return new WaitForSeconds(shuffleTime);
                deck.Shuffle();
                yield return new WaitForSeconds(drawTime);
                spell = deck.DrawSpell();
            }
            slots.Equip(spell);
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

    public class EquipmentSlot
    {
        public Dictionary<SpellSlot, Spell?> currentSpells = new Dictionary<SpellSlot, Spell?>();

        public EquipmentSlot()
        {
            currentSpells.Add(SpellSlot.Spell1, null);
            currentSpells.Add(SpellSlot.Spell2, null);
            currentSpells.Add(SpellSlot.Spell3, null);
        }

        public SpellSlot? GetEmptySlot()
        {
            if (currentSpells[SpellSlot.Spell1] == null) return SpellSlot.Spell1;
            if (currentSpells[SpellSlot.Spell2] == null) return SpellSlot.Spell2;

            if (currentSpells[SpellSlot.Spell3] == null) return SpellSlot.Spell3;
            return null;
        }

        public SpellSlot? Equip(Spell spell)
        {
            var slot = GetEmptySlot();
            if (slot == null) return null;
            currentSpells[(SpellSlot)slot] = spell;
            return slot;
        }

        public Spell? GetSpell(SpellSlot slot)
        {
            return currentSpells[slot];
        }
    }
}
