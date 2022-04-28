using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorldMap
{
    public class Room
    {
        public Vector2 center
        {
            get
            {
                return new Vector2(tiles.Count / 2, tiles[0].Count / 2) + offset;
            }
        }

        public int size
        {
            get
            {
                return tiles.GetSize();
            }
        }

        public float Distance(Room room)
        {
            return (room.center - center).magnitude;
        }

        private List<List<TileContainer>> tiles;
        private Vector2 offset;

        public Room(List<List<TileContainer>> tiles, Vector2? offset)
        {
            this.offset = offset ?? Vector2.zero;
            this.tiles = tiles;
            for (int x = 0; x < tiles.Count; x++)
            {
                for (int y = 0; y < tiles[x].Count; y++)
                {
                    tiles[x][y].tile = new Floor();
                }
            }

            var north = tiles.FirstOrDefault();
            foreach (int index in Enumerable.Range(0, north.Count))
            {
                north[index].tile = new NorthWall();
            }

            var south = tiles.LastOrDefault();
            foreach (int index in Enumerable.Range(0, south.Count))
            {
                south[index].tile = new SouthWall();
            }

            foreach (int index in Enumerable.Range(0, tiles.Count))
            {
                tiles[index][0].tile = new HorizontalWall();
                tiles[index][tiles[index].Count - 1].tile = new HorizontalWall();
            }

            Vector2 c = (this.center - offset) ?? Vector2.zero;
            Debug.Log(c + " " + tiles.Count + " " + tiles[0].Count);
            if (c != null)
                tiles[(int)c.x][(int)c.y].tile = new Empty();
        }

        // 中心を結んだ線を対角線とする四角形を作成する
        public Rect CenterToCenter(Room other)
        {
            return new Rect(
                Mathf.Min(center.x, other.center.x),
                Mathf.Min(center.y, other.center.y),
                Mathf.Max(Mathf.Abs(center.x - other.center.x), 1),
                Mathf.Max(Mathf.Abs(center.y - other.center.y), 1)
            );
        }

        public void MakeGate()
        {

        }
    }
}