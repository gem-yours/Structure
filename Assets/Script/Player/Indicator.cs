using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#nullable enable
public class Indicator : MonoBehaviour
{
    private static readonly float draggingThreshold = 75;
#pragma warning disable CS8618
    public GameObject directionIndicator;
#pragma warning restore CS8618
    public Vector2 direction { get; private set; } = Vector2.right;

    private bool isOverThreshold = false;


    public bool IsActive(Spell.TargetType targetType)
    {
        switch (targetType)
        {
            case Spell.TargetType.Auto:
                return true;
            case Spell.TargetType.Direction:
                return directionIndicator.activeSelf;
        }
        return false;
    }

    public void IndicateDirection(Vector2 direction)
    {
        this.direction = direction;

        if (direction == Vector2.zero)
        {
            return;
        }
        if (isOverThreshold && direction.magnitude < draggingThreshold)
        {
            directionIndicator.SetActive(false);
            return;
        }
        if (direction.magnitude >= draggingThreshold)
        {
            isOverThreshold = true;
        }
        directionIndicator.SetActive(true);
        directionIndicator.transform.rotation = Quaternion.FromToRotation(Vector2.up, direction);
        var size = 0.2f;
        directionIndicator.transform.localScale = new Vector3(
            size,
            (1 - Mathf.Abs(direction.normalized.y)) * size * 0.4f + size * 0.6f,
            1
        );
    }

    public void HideIndicator()
    {
        directionIndicator.SetActive(false);
        isOverThreshold = false;
    }
}
