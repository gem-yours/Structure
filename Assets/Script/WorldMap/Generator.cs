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
    }
}