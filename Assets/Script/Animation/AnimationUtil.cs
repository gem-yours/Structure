using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


public static class AlphaExtension
{
    public static IEnumerator AppearFromAlpha(this Image image, float animationDuration)
    {
        yield return AnimationUtil.EaseInOut(animationDuration, (float current) =>
        {
            var color = image.color;
            color.a = 1 - current;
            image.color = color;
        });
    }

    public static IEnumerator DisppearToAlpha(this Image image, float animationDuration)
    {
        yield return AnimationUtil.EaseInOut(animationDuration, (float current) =>
        {
            var color = image.color;
            color.a = current;
            image.color = color;
        });
    }
}