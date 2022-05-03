using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
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

        // TODO: Groundクラスでタイルを管理する
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

            var leading = tiles.FirstOrDefault();
            foreach (int index in Enumerable.Range(0, leading.Count))
            {
                leading[index].tile = new VerticalWall();
            }

            var trailing = tiles.LastOrDefault();
            foreach (int index in Enumerable.Range(0, trailing.Count))
            {
                trailing[index].tile = new VerticalWall();
            }

            foreach (int index in Enumerable.Range(0, tiles.Count))
            {
                tiles[index][0].tile = new SouthWall();
                tiles[index][tiles[index].Count - 1].tile = new NorthWall();
            }
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

        public bool isContain(TileContainer container)
        {
            return GetPosition(container) == null;
        }

        public Vector2? GetPosition(TileContainer container)
        {
            foreach (int x in Enumerable.Range(0, tiles.Count))
            {
                foreach (int y in Enumerable.Range(0, tiles[x].Count))
                {
                    if (tiles[x][y] == container)
                    {
                        return new Vector2(x, y);
                    }
                }
            }
            return null;
        }

        public List<TileContainer>? GetTerrain(TileContainer container)
        {
            // tmpがunwrapされないので別の変数を用意する
            var tmp = GetPosition(container);
            if (tmp == null) return null;
            var position = (Vector2)tmp;

            if (container.Equals(new SouthWall()) ||
                container.Equals(new NorthWall()))
            {
                var terrain = new List<TileContainer>();
                foreach (int x in Enumerable.Range(0, tiles.Count))
                {
                    terrain.Add(tiles[x][(int)position.y]);
                }
                return terrain;
            }
            if (container.Equals(new VerticalWall()))
            {
                // TODO: 端っこを含まないようにする
                return tiles[(int)position.x];
            }
            return null;
        }

        public void MakeGate()
        {

        }
    }
}