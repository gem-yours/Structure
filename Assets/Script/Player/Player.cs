using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public enum SpellSlot
{
    Attack,
    Spell1,
    Spell2,
    Spell3,
    Unique
}
public class Player : MonoBehaviour
{
    public ExpManager expManager { private set; get; } = new ExpManager();
    public float speed { private set; get; } = 0.15f;
    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }

    public Deck deck = new Deck();

    private Vector2 movingDirection = Vector2.zero;
    private Rigidbody2D rb2D;
    private Animator animator;

    public void ChangeMoveDirection(Vector2 direction)
    {
        movingDirection = direction;

        animator.SetBool("isWalking", direction != Vector2.zero);
    }

    private void Move()
    {
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

    public void Attack(SpellSlot spellSlot)
    {
        var boltObject = Instantiate(Resources.Load("Effects/Firebolt"), transform.position, transform.rotation) as GameObject;
        var bolt = boltObject.GetComponent<BoltProjectile>();
        bolt.spell = new FireBolt();
        bolt.Target(EnemiesManager.instance.NearestEnemy(transform.position));
    }

    private IEnumerator StartCast(Spell spell)
    {
        for (int time = 0; time < spell.magazine; time++)
        {
            Debug.Log(deck.remaingSpells.Count);
            yield return new WaitForSeconds(spell.delay);
        }
        DrawSpell();
    }

    private void DrawSpell()
    {
        StartCoroutine(StartCast(deck.DrawSpell()));
    }

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        List<Spell> initialSpells = Enumerable.Repeat(new FireBolt() as Spell, 3).ToList();
        deck.AddSpells(initialSpells);

        // for (int i = 0; i < 4; i++)
        // {
        DrawSpell();
        // }
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
