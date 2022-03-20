using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static void Generate(Vector2 center, int column, int row, GameObject tile, GameObject wall)
    {
        var size = tile.GetComponent<SpriteRenderer>().size;

        var tileTip = new Vector2(column, row) / 2;
        // 床を生成
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                var position = Vector2.Scale(new Vector2(x, y) - tileTip, size);
                Instantiate(tile, position + center, Quaternion.identity);
            }
        }

        var numberOfWall = new Vector2(10, 10);
        var wallTip = tileTip + numberOfWall;

        // 右側の壁
        GenerateWallEdge(
            wallTip,
            Vector2.Scale(tileTip, new Vector2(1, -1)),
            wall,
            size
        );
        // 下側の壁
        GenerateWallEdge(
            Vector2.Scale(wallTip, new Vector2(1, -1)),
            tileTip * -1,
            wall,
            size
        );
        // 左側の壁
        GenerateWallEdge(
            wallTip * -1,
            Vector2.Scale(tileTip, new Vector2(-1, 1)),
            wall,
            size
        );
        // 上側の壁
        GenerateWallEdge(
            Vector2.Scale(wallTip, new Vector2(-1, 1)),
            tileTip,
            wall,
            size
        );
    }

    /// 指定した範囲を壁で塗りつぶす
    private static void GenerateWallEdge(Vector2 start, Vector2 end, GameObject wall, Vector2 size)
    {
        
        for (float x = Mathf.Min(start.x, end.x); x < Mathf.Max(start.x, end.x); x++)
        {
            for (float y = Mathf.Min(start.y, end.y); y < Mathf.Max(start.y, end.y); y++)
            {
                var position = Vector2.Scale(new Vector2(x, y), size);
                Instantiate(wall, position, Quaternion.identity);
            }
        }
    }
}
