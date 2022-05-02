using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace WorldMap
{
    public class Generator : MonoBehaviour
    {
        public static void Generate(LocalArea area, Vector2 offset)
        {
            foreach (int x in Enumerable.Range(0, area.ground.column))
            {
                foreach (int y in Enumerable.Range(0, area.ground.row))
                {
                    var obj = area.ground.Get(x, y)?.Resource();
                    if (obj == null) continue;
                    // タイルの中心を中央から左下にするため+0.5している
                    Instantiate(obj, offset + new Vector2(x + 0.5f, y + 0.5f), Quaternion.identity);
                }
            }

        }
    }
}