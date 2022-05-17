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

#pragma warning disable CS8618
    private Texture2D texture;
    private Image image;

#pragma warning restore CS8618
    public void DrawMap(Vector2 center, Vector2 playerPosition)
    {
        if (ground == null) return;

        foreach (var x in Enumerable.Range(0, texture.width))
        {
            foreach (var y in Enumerable.Range(0, texture.height))
            {
                var tile = ground.Get(
                    (int)(x - resolution / 2 + center.x + ground.columns / 2),
                    (int)(y - resolution / 2 + center.y + ground.rows / 2)
                );

                var position = new Vector2(x, y);
                if (tile == null)
                {
                    DrawRect(position, 1, new Color(0, 0, 0, 0));
                    continue;
                }

                var color = new Color(0, 0, 0, 0);
                if (tile.Equals(new NorthWall()) ||
                    tile.Equals(new SouthWall()) ||
                    tile.Equals(new VerticalWall()))
                {
                    color = new Color(1, 1, 1);
                }
                if (tile.Equals(new Floor()))
                {
                    color = new Color(0, 0, 1, 0.5f);
                }
                if (tile.Equals(new Empty()))
                {
                    color = new Color(0, 0, 0, 0);
                }
                DrawRect(position, 1, color);
            }
        }

        var playerPositionInMiniMap = playerPosition - center + new Vector2(resolution / 2, resolution / 2);
        DrawRect(playerPositionInMiniMap, 4, new Color(1, 0, 0));
        texture.Apply();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }


    private void DrawRect(Vector2 center, int diameter, Color color)
    {
        foreach (var x in Enumerable.Range(-diameter / 2, diameter))
        {
            foreach (var y in Enumerable.Range(-diameter / 2, diameter))
            {
                var position = center + new Vector2(x, y);
                if (
                    position.x < 0 || position.x > texture.width ||
                    position.y < 0 || position.y > texture.height
                )
                {
                    continue;
                }
                texture.SetPixel((int)position.x, (int)position.y, color);
            }
        }
    }

    private void Start()
    {
        image = GetComponent<Image>();
        var rectTransform = GetComponent<RectTransform>();
        // 幅はaspect ratio fitterによって制御されている
        var mapSize = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x) * 2;
        texture = new Texture2D((int)resolution, (int)resolution);
    }

    private Vector2 Abs(Vector2 vector)
    {
        return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }
}
