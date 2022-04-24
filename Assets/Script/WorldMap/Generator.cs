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
            foreach (int x in Enumerable.Range(0, area.tiles.Count))
            {
                foreach (int y in Enumerable.Range(0, area.tiles[x].Count))
                {
                    var obj = area.tiles[x][y].Resource();
                    if (obj == null) continue;
                    Instantiate(obj, offset + new Vector2(y, x), Quaternion.identity);
                }
            }

        }
    }
}