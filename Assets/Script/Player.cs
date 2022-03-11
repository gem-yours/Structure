using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 1f;
    
    private Rigidbody2D rb2D;
    private Animator animator;

    public void AttemptMove(Vector2 direction)
    {
        var current = rb2D.position;
        rb2D.MovePosition(Vector2.MoveTowards(current, current + direction, speed));

        animator.SetBool("isWalking", true);

        var leftOrRight = (direction.x < 0) ? 1 : -1; 
        if (leftOrRight != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x) * leftOrRight, 
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

    }
}
