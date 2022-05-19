using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#nullable enable
public class Indicator : MonoBehaviour
{
#pragma warning disable CS8618
    public GameObject directionIndicator;
#pragma warning restore CS8618
    public Vector2 direction { get; private set; } = Vector2.up;

    private float draggingThreshold = 75;


    public void IndicateDirection(Vector2 direction)
    {
        this.direction = direction;

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

}
