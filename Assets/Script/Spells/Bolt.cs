using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    public Vector2 direction = Vector2.left;
    public float speed = 0.5f;
    private Rigidbody2D rb2D;

    public void Target(GameObject target)
    {
        if (target == null)
        {
            direction = Vector2.left;            
        }
        direction = (target.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void Move()
    {
        var current = rb2D.position;
        rb2D.MovePosition(Vector2.MoveTowards(current, current + direction, speed));


        rb2D.transform.rotation =  Quaternion.FromToRotation(Vector3.right, direction);
    }

    protected virtual IEnumerator Fade()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        StartCoroutine(Fade());
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Move();
    }
}