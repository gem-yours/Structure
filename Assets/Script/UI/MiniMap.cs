using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WorldMap;

#nullable enable
public class MiniMap : MonoBehaviour
{
#pragma warning disable CS8618
    public Canvas canvas;

#pragma warning restore CS8618
    public Ground? ground;

#pragma warning disable CS8618

    private Vector2 mapSize;
#pragma warning restore CS8618

    public void DrawMap(Vector2 positiion, int resolution = 25)
    {
        RemoveChildren();
        if (ground == null) return;
        var unit = mapSize / resolution;
        foreach (var x in Enumerable.Range(0, resolution))
        {
            foreach (var y in Enumerable.Range(0, resolution))
            {
                var tile = ground.Get(
                    (int)(x - resolution / 2 + positiion.x + ground.columns / 2),
                    (int)(y - resolution / 2 + positiion.y + ground.rows / 2)
                );
                if (tile == null) continue;

                // 中心を左下にする
                var position = Vector2.Scale(new Vector2(x - resolution / 2, y - resolution / 2), unit);
                if (tile.Equals(new NorthWall()) ||
                    tile.Equals(new SouthWall()) ||
                    tile.Equals(new VerticalWall()))
                {
                    DrawRect(
                        position,
                        unit * 0.05f, // TODO: 単位を合わせる
                        new Color(1, 1, 1)
                    );
                }
                if (tile.Equals(new Floor()))
                {
                    DrawRect(
                        position,
                        unit * 0.05f, // TODO: 単位を合わせる
                        new Color(0, 0, 1)
                    );
                }
            }
        }
    }


    private void RemoveChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }


    private void DrawRect(Vector2 position, Vector2 size, Color color)
    {
        var rect = new GameObject("MapContents");
        rect.transform.SetParent(this.transform);
        var rectTransform = rect.AddComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.localPosition = position;

        var image = rect.AddComponent<Image>();
        image.color = color;
        // TODO: 画像を追加
    }

    private void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        // 高さはaspect ratio fitterによって制御されている
        mapSize = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x) * 2;
    }
}
