using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SpellSlot {
    Attack,
    Spell1,
    Spell2,
    Spell3,
    Unique
}
public class Player : MonoBehaviour
{
    public float speed;
    
    private Vector2 movingDirection = Vector2.zero;
    private Rigidbody2D rb2D;
    private Animator animator;

    public void ChangeMoveDirection(Vector2 direction)
    {
        movingDirection = direction;
        
        animator.SetBool("isWalking", direction != Vector2.zero);
    }

    public void Cast(SpellSlot spellSlot)
    {
        Instantiate(Resources.Load("Effects/Firebolt"),transform.position, transform.rotation);
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

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movingDirection != Vector2.zero) {
            Move();
        }
    }
}
