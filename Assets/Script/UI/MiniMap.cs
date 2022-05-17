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
    public int resolution = 50;

    private List<List<GameObject>> gameObjects = new List<List<GameObject>>();

    public void DrawMap(Vector2 center, Vector2 playerPosition)
    {
        if (ground == null) return;
        foreach (var x in Enumerable.Range(0, gameObjects.Count))
        {
            foreach (var y in Enumerable.Range(0, gameObjects[x].Count))
            {
                var tile = ground.Get(
                    (int)(x - resolution / 2 + center.x + ground.columns / 2),
                    (int)(y - resolution / 2 + center.y + ground.rows / 2)
                );
                if (tile == null)
                {
                    gameObjects[x][y].GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    continue;
                }

                if (tile.Equals(new NorthWall()) ||
                    tile.Equals(new SouthWall()) ||
                    tile.Equals(new VerticalWall()))
                {
                    gameObjects[x][y].GetComponent<Image>().color = new Color(1, 1, 1);
                }
                if (tile.Equals(new Floor()))
                {
                    gameObjects[x][y].GetComponent<Image>().color = new Color(0, 0, 1, 0.5f);
                }
                if (tile.Equals(new Empty()))
                {
                    gameObjects[x][y].GetComponent<Image>().color = new Color(0, 0, 0, 0);
                }
            }
        }

        var playerPositionInMiniMap = playerPosition - center + new Vector2(resolution / 2, resolution / 2);
        DrawRect(playerPositionInMiniMap, 1, new Color(1, 0, 0));
    }


    private void DrawRect(Vector2 center, int radius, Color color)
    {
        foreach (var x in Enumerable.Range(-radius, radius * 2))
        {
            foreach (var y in Enumerable.Range(-radius, radius * 2))
            {
                var position = center + new Vector2(x, y);
                if (
                    position.x < 0 || position.x > gameObjects.Count ||
                    position.y < 0 || position.y > gameObjects[0].Count
                )
                {
                    continue;
                }
                gameObjects[(int)position.x][(int)position.y].GetComponent<Image>().color = color;
            }
        }
    }

    private void PrepareObjects(Vector2 mapSize)
    {
        gameObjects = Enumerable.Range(0, resolution).Select(x =>
        {
            return Enumerable.Range(0, resolution).Select(y =>
            {
                var rect = new GameObject("MapContents");
                rect.layer = gameObject.layer;
                rect.transform.SetParent(this.transform);
                var rectTransform = rect.AddComponent<RectTransform>();
                rectTransform.localScale = Vector2.one;
                rectTransform.sizeDelta = Abs(mapSize / resolution);
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
        // 幅はaspect ratio fitterによって制御されている
        var mapSize = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x) * 2;
        PrepareObjects(mapSize);
    }

    private Vector2 Abs(Vector2 vector)
    {
        return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }
}
