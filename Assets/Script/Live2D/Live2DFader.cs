using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Rendering;


# nullable enable
public class Live2DFader : MonoBehaviour
{
    private CubismRenderController? renderontroller;
    private Living? living;

    void Start()
    {
        renderontroller = GetComponent<CubismRenderController>();
        living = GetComponent<Living>();
        living.damageAnimation = () =>
        {
            return Luminous();
        };
        living.deadAnimation = () =>
        {
            return Fade();
        };
    }

    public IEnumerator Luminous()
    {
        yield return null;
    }

    public IEnumerator Fade()
    {
        if (renderontroller == null) yield break;

        var animationDuration = 0.25f;
        for (float current = 0; current < animationDuration; current += Time.deltaTime)
        {
            foreach (CubismRenderer rendere in renderontroller.Renderers)
            {
                var color = rendere.Color;
                color.a = 1 - current / animationDuration;
                rendere.Color = color;
            }
            yield return null;
        }
    }
}
