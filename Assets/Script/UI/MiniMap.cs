using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WorldMap;

#nullable enable
public class MiniMap : MonoBehaviour
{
    public Ground? ground
    {
        set
        {
            if (value == null) return; // TODO: マップを消去する

            DrawMap(value);
        }
    }

#pragma warning disable CS8618
    private Vector2 mapSize;
#pragma warning restore CS8618

    private void DrawMap(Ground ground)
    {
        var unit = Vector2.Scale(mapSize, new Vector2(1f / ground.columns, 1f / ground.rows));
        foreach (var x in Enumerable.Range(0, ground.columns))
        {
            foreach (var y in Enumerable.Range(0, ground.rows))
            {
                var tile = ground.Get(x, y);
                if (tile == null) continue;
                if (tile.Equals(new NorthWall()) ||
                    tile.Equals(new NorthWall()) ||
                    tile.Equals(new NorthWall()))
                {
                    // 中心を左下にする
                    DrawRect(
                        Vector2.Scale(new Vector2(x - ground.columns / 2, y - ground.rows / 2), unit),
                        unit
                    );
                }
            }
        }
    }


    private void DrawRect(Vector2 position, Vector2 size)
    {
        var rect = new GameObject("MapContents");
        rect.transform.SetParent(this.transform);
        var rectTransform = rect.AddComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.localPosition = position;

        var image = rect.AddComponent<Image>();
        // TODO: 画像を追加
    }

    private void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        // 高さはaspect ratio fitterによって制御されている
        mapSize = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x);
    }
}
