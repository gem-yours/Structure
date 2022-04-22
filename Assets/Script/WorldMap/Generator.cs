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
                    Instantiate(area.tiles[x][y].Resource(), offset + new Vector2(y, x), Quaternion.identity);
                }
            }

        }
    }
}