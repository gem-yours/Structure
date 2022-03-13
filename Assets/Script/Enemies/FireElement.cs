using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElement : Enemy
{
    public float speed;

    private void MoveToPlayer()
    {
        Vector2 direction = (GameManager.instance.playerRb2D.transform.position - rb2D.transform.position).normalized;
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
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
        MoveToPlayer();
    }
}
