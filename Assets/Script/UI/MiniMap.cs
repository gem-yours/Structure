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

    private int _resolution = 50;
    public int resolution
    {
        get
        {
            return _resolution;
        }
        set
        {
            if (value == _resolution) return;
            _resolution = value;
            texture = new Texture2D(value, value);
        }
    }

    private Vector2 center = Vector2.zero;

    private Texture2D? texture;
    private Image? image;

    private readonly Color floorColor = ColorCreator.Hex(0x6079B5).withAlpha(0.5f);
    private readonly Color wallColor = ColorCreator.Hex(0xAEAECF);
    private readonly Color playerColor = ColorCreator.Hex(0xE05562);
    private readonly Color enemyColor = ColorCreator.Hex(0x6EE055);


    public void DrawMap(Vector2 center, Vector2 playerPosition)
    {
        this.center = center;
        if (ground is null || texture is null) return;

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
                    color = wallColor;
                }
                if (tile.Equals(new Floor()))
                {
                    color = floorColor;
                }
                if (tile.Equals(new Empty()))
                {
                    color = new Color(0, 0, 0, 0);
                }
                DrawRect(position, 1, color);
            }
        }

        var playerPositionInMiniMap = RealPositionToMapPosition(playerPosition);
        DrawRect(playerPositionInMiniMap, 4, playerColor);
    }


    public void AddEnemy(List<Vector2> enemies)
    {
        foreach (var enemy in enemies)
        {
            DrawRect(
                RealPositionToMapPosition(enemy),
                4,
                enemyColor
            );
        }
    }


    public void Apply()
    {
        if (texture is null || image is null) return;
        texture.Apply();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    private Vector2 RealPositionToMapPosition(Vector2 realPosition)
    {
        return realPosition - center + new Vector2(resolution / 2, resolution / 2);
    }


    private void DrawRect(Vector2 center, int diameter, Color color)
    {
        if (texture is null) return;
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


// https://www.create-forever.games/unity-color-hexadecimal/
public class ColorCreator
{
    /// <summary>
    /// RGB を 0 ～ 255 で指定したカラー値を取得
    /// </summary>
    /// <param name="r">赤</param>
    /// <param name="g">緑</param>
    /// <param name="b">青</param>
    public static Color Rgb(int r, int g, int b)
    {
        return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f);
    }
    /// <summary>
    /// カラーを #RRGGBB の形で取得
    /// </summary>
    /// <param name="hexrgb">16進数のカラー値 RRGGBB</param>
    public static Color Hex(int hexrgb)
    {
        int r = (hexrgb >> 16) & 0xff;
        int g = (hexrgb >> 8) & 0xff;
        int b = hexrgb & 0xff;
        return Rgb(r, g, b);
    }
}

public static class ColorWithAlpha
{
    public static Color withAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
}