using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace WorldMap
{

    public class Ground
    {
        public int column
        {
            get
            {
                return tiles.Count;
            }
        }


        public int row
        {
            get
            {
                return tiles[0].Count;
            }
        }

        public int size
        {
            get
            {
                return column * row;
            }
        }

        public List<List<TileContainer>> tiles;

        public Ground(List<List<TileContainer>> tiles)
        {
            this.tiles = tiles;
        }

        public List<TileContainer>? GetRow(int x)
        {
            if (x < 0 || x > tiles.Count)
            {
                return null;
            }
            return tiles[x];
        }

        public TileContainer? Get(int x, int y)
        {
            if (x < 0 || x > tiles.Count ||
                y < 0 || y > tiles[x].Count)
            {
                return null;
            }
            return tiles[x][y];
        }

        public Vector2 GetOffset<TileContainer>(TileContainer target)
        {
            foreach (int x in Enumerable.Range(0, column))
            {
                foreach (int y in Enumerable.Range(0, row))
                {
                    var tmp = Get(x, y);
                    if (tmp == null) continue;
                    if (tmp.Equals(target))
                    {
                        return new Vector2(x, y);
                    }
                }
            }
            return Vector2.zero;
        }

        public (Ground, Ground)? SplitArea(Direction direction)
        {
            var minimumEdge = 7;
            switch (direction)
            {
                case Direction.Column:
                    {
                        if (column < minimumEdge * 2)
                        {
                            break;
                        }
                        var border = Random.Range(minimumEdge, column - 1 - minimumEdge);
                        var first = tiles.GetRange(0, border);
                        var second = tiles.GetRange(border, column - border);
                        return (new Ground(first), new Ground(second));
                    }
                case Direction.Row:
                    {
                        if (row < minimumEdge * 2)
                        {
                            break;
                        }
                        var border = Random.Range(minimumEdge, row - 1 - minimumEdge);
                        var first = new List<List<TileContainer>>();
                        var second = new List<List<TileContainer>>();
                        for (int i = 0; i < column; i++)
                        {
                            var tileRow = GetRow(i);
                            if (tileRow == null) continue;
                            first.Add(tileRow.GetRange(0, border));
                            second.Add(tileRow.GetRange(border, row - border));
                        }
                        return (new Ground(first), new Ground(second));
                    }
            }

            return null;
        }

    }

    public enum Direction
    {
        Column,
        Row
    }
}