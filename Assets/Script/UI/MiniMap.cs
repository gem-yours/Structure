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

    private List<List<GameObject>> gameObjects = new List<List<GameObject>>();
    private int resolution = 50;
#pragma warning disable CS8618

    private Vector2 mapSize;
#pragma warning restore CS8618

    public void DrawMap(Vector2 positiion)
    {
        if (ground == null) return;
        foreach (var x in Enumerable.Range(0, gameObjects.Count))
        {
            foreach (var y in Enumerable.Range(0, gameObjects[x].Count))
            {
                var tile = ground.Get(
                    (int)(x - resolution / 2 + positiion.x + ground.columns / 2),
                    (int)(y - resolution / 2 + positiion.y + ground.rows / 2)
                );
                if (tile == null) continue;

                if (tile.Equals(new NorthWall()) ||
                    tile.Equals(new SouthWall()) ||
                    tile.Equals(new VerticalWall()))
                {
                    gameObjects[x][y].GetComponent<Image>().color = new Color(1, 1, 1);
                }
                if (tile.Equals(new Floor()))
                {
                    gameObjects[x][y].GetComponent<Image>().color = new Color(0, 0, 1);
                }
                if (tile.Equals(new Empty()))
                {
                    gameObjects[x][y].GetComponent<Image>().color = new Color(0, 0, 0, 0);
                }
            }
        }
    }

    private void PrepareObjects()
    {
        gameObjects = Enumerable.Range(0, resolution).Select(x =>
        {
            return Enumerable.Range(0, resolution).Select(y =>
            {
                var rect = new GameObject("MapContents");
                rect.transform.SetParent(this.transform);
                var rectTransform = rect.AddComponent<RectTransform>();
                // TODO: 正しいサイズを設定する
                rectTransform.localScale = Vector2.one;
                rectTransform.sizeDelta = mapSize / resolution;
                rectTransform.localPosition = Vector2.Scale(
                    new Vector2(x - resolution / 2, y - resolution / 2),
                    mapSize / resolution
                );
                var image = rect.AddComponent<Image>();
                image.color = new Color(0, 0, 0, 1);

                return rect;
            }).ToList();
        }).ToList();
    }

    private void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        // 高さはaspect ratio fitterによって制御されている
        mapSize = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x) * 2;
        PrepareObjects();
    }
}
