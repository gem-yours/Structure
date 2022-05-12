using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldMap;

#nullable enable
public class MiniMap : MonoBehaviour
{
    public Ground? ground
    {
        set
        {
            if (value == null) return; // TODO: マップを消去する

            DrawMap(value, size / value.columns);
        }
    }

#pragma warning disable CS8618
    private Vector2 size;
#pragma warning restore CS8618

    private void DrawMap(Ground ground, Vector2 size)
    {
        foreach (var x in Enumerable.Range(0, ground.columns))
        {
            foreach (var y in Enumerable.Range(0, ground.rows))
            {
                if (ground.tiles.Equals(new NorthWall()) ||
                    ground.tiles.Equals(new NorthWall()) ||
                    ground.tiles.Equals(new NorthWall()))
                {
                    var wall = new GameObject("wall" + x + y);
                    wall.transform.SetParent(this.transform);
                    wall.transform.localScale = size;
                }
            }
        }
    }


    private void DrawRect()
    {

    }

    private void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        // 高さはaspect ratio fitterによって制御されている
        size = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x);
    }
}
