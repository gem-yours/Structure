using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace WorldMap
{
    public class Generator : MonoBehaviour
    {
        public static void Generate(LocalArea area)
        {
            foreach (int x in Enumerable.Range(0, area.ground.columns))
            {
                foreach (int y in Enumerable.Range(0, area.ground.rows))
                {
                    var obj = area.ground.Get(x, y)?.Resource();
                    if (obj == null) continue;
                    // タイルの中心を中央から左下にするため+0.5している
                    Instantiate(obj, area.offset + new Vector2(x + 0.5f, y + 0.5f), Quaternion.identity);
                }
            }

        }


        public static void CreateOuterWall(Rect rect)
        {
            var offset = new Vector2(0.5f, 0.5f);
            var tile = new TileContainer(new Empty());
            foreach (int x in Enumerable.Range((int)rect.xMin, (int)rect.width))
            {
                Instantiate(tile.Resource(), new Vector2(x, rect.yMin) + offset, Quaternion.identity);
                Instantiate(tile.Resource(), new Vector2(x, rect.yMax) + offset, Quaternion.identity);
            }
            foreach (int y in Enumerable.Range((int)rect.yMin, (int)rect.height))
            {
                Instantiate(tile.Resource(), new Vector2(rect.xMin, y) + offset, Quaternion.identity);
                Instantiate(tile.Resource(), new Vector2(rect.xMax, y) + offset, Quaternion.identity);
            }
        }
    }
}