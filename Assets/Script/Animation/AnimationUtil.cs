using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public class AnimationUtil
{
    public delegate void AnimationDelegate(float current);

    public static IEnumerator Linear(float animationDuration, AnimationDelegate animationDelegate)
    {
        for (float current = 0; current <= animationDuration; current += Time.deltaTime)
        {
            animationDelegate(current / animationDuration);
            yield return null;
        }
        animationDelegate(animationDuration);
    }
    public static IEnumerator EaseInOut(float animationDuration, AnimationDelegate animationDelegate)
    {
        var animationCurve = AnimationCurve.EaseInOut(0, 1, animationDuration, 0);
        for (float current = 0; current <= animationDuration; current += Time.deltaTime)
        {
            animationDelegate(animationCurve.Evaluate(current));
            yield return null;
        }
        animationDelegate(animationCurve.Evaluate(animationDuration));
    }
}


public sealed class WaitForSecondsRealtime : CustomYieldInstruction
{
    private float waitUntilThis;

    public override bool keepWaiting
    {
        get { return Time.realtimeSinceStartup < waitUntilThis; }
    }

    public WaitForSecondsRealtime(float waithingTime)
    {
        waitUntilThis = Time.realtimeSinceStartup + waithingTime;
    }
}