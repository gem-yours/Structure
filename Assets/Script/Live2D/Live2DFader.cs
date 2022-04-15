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
        if (renderontroller == null) yield break;

        var animationDuration = 0.1f;

        var colors = new List<Color>();
        foreach (CubismRenderer render in renderontroller.Renderers)
        {
            colors.Add(render.Color);
            render.Color = new Color(1, 0, 0, 1);
        }

        yield return new WaitForSeconds(animationDuration);

        foreach (CubismRenderer render in renderontroller.Renderers)
        {
            render.Color = new Color(1, 1, 1, 1);
        }

    }

    public IEnumerator Fade()
    {
        if (renderontroller == null) yield break;

        var animationDuration = 0.25f;
        for (float current = 0; current < animationDuration; current += Time.deltaTime)
        {
            foreach (CubismRenderer render in renderontroller.Renderers)
            {
                var color = render.Color;
                color.a = 1 - current / animationDuration;
                render.Color = color;
            }
            yield return null;
        }
    }
}
