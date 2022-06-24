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
                return new Vector2(ground.columns / 2, ground.rows / 2) + offset;
            }
        }

        public int size
        {
            get
            {
                return ground.size;
            }
        }

        public Rect rect
        {
            get
            {
                return new Rect(offset.x, offset.y, ground.columns, ground.rows);
            }
        }

        public float Distance(Room room)
        {
            return (room.center - center).magnitude;
        }

        public Ground ground { private set; get; }
        public Vector2 offset { private set; get; }

        public Room(List<List<TileContainer>> tiles, Vector2? offset)
        {
            this.offset = offset ?? Vector2.zero;
            ground = new Ground(tiles);
            foreach (int x in Enumerable.Range(0, ground.columns))
            {
                foreach (int y in Enumerable.Range(0, ground.rows))
                {
                    ground.Set(x, y, new Floor());
                }
            }

            foreach (int y in Enumerable.Range(0, ground.rows))
            {
                ground.Set(0, y, new VerticalWall());
                ground.Set(ground.columns - 1, y, new VerticalWall());
            }

            foreach (int x in Enumerable.Range(0, ground.columns))
            {
                ground.Set(x, 0, new SouthWall());
                ground.Set(x, ground.rows - 1, new NorthWall());
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
            foreach (int x in Enumerable.Range(0, ground.columns))
            {
                foreach (int y in Enumerable.Range(0, ground.rows))
                {
                    if (ground.Get(x, y) == container)
                    {
                        return new Vector2(x, y);
                    }
                }
            }
            return null;
        }

        // 指定した壁に連続する壁をすべて取得する
        public List<TileContainer>? GetTerrain(TileContainer container)
        {
            // tmpがunwrapされないので別の変数を用意する
            var tmp = GetPosition(container);
            if (tmp == null) return null;
            var position = (Vector2)tmp;

            if (container.Equals(new SouthWall()) ||
                container.Equals(new NorthWall()))
            {
                return ground.GetColumn((int)position.y);
            }
            if (container.Equals(new VerticalWall()))
            {
                // TODO: 端っこを含まないようにする
                ground.GetRow((int)position.x);
            }
            return null;
        }
    }
}