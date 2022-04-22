using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

#nullable enable
namespace WorldMap
{
    public class LocalArea
    {
        public List<Room> rooms = new List<Room>();
        public int roomsNumber = 1;


        private List<List<TileContainer>> tiles;
        private int maxRoomsNumber;

        public LocalArea(int columns, int rows, int maxRoomsNumber = 100, int minimumAreaSize = 0)
        {
            this.maxRoomsNumber = maxRoomsNumber;
            tiles = Enumerable.Range(0, columns).Select(x =>
            {
                return Enumerable.Range(0, rows).Select(x => new TileContainer(new Empty())).ToList();
            }).ToList();

            TryToCreateRooms(tiles, minimumAreaSize, Direction.Column);
        }

        private void TryToCreateRooms(List<List<TileContainer>> localArea, int minimumArea, Direction direction)
        {
            // 部屋の数が最大数に達している場合はこれ以上分割しない
            if (roomsNumber >= maxRoomsNumber)
            {
                CreateRoom(localArea);
                return;
            }

            var (area1, area2) = SplitArea(localArea, direction) ?? default;

            // 分割に失敗したか、分割した結果最小の面積を下回った場合は分割せず部屋を作成する
            // TODO: 最小の面積を下回った場合、分割位置を変えることで分割できないか確かめる
            if (area1 == null && area2 == null ||
                area1?.GetArea() < minimumArea || area2?.GetArea() < minimumArea)
            {
                CreateRoom(localArea);
                return;
            }

            // 分割に成功すると部屋の数が1つ増える
            roomsNumber++;

            if (area1 != null)
            {
                TryToCreateRooms(area1, minimumArea, direction.Reverse());
            }
            if (area2 != null)
            {
                TryToCreateRooms(area2, minimumArea, direction.Reverse());
            }
        }

        private (List<List<TileContainer>>, List<List<TileContainer>>)? SplitArea(List<List<TileContainer>> map, Direction direction)
        {
            var minimumEdge = 4;
            switch (direction)
            {
                case Direction.Column:
                    {
                        if (map.Count < minimumEdge * 2)
                        {
                            break;
                        }
                        var border = Random.Range(minimumEdge, map.Count - 1 - minimumEdge);
                        var first = map.GetRange(0, border);
                        var second = map.GetRange(border, map.Count - border);
                        return (first, second);
                    }
                case Direction.Row:
                    {
                        var row = map.FirstOrDefault();
                        if (row.Count < minimumEdge * 2)
                        {
                            break;
                        }
                        var border = Random.Range(minimumEdge, row.Count - 1 - minimumEdge);
                        var first = new List<List<TileContainer>>();
                        var second = new List<List<TileContainer>>();
                        for (int i = 0; i < map.Count; i++)
                        {
                            first.Add(map[i].GetRange(0, border));
                            second.Add(map[i].GetRange(border, row.Count - border));
                        }
                        return (first, second);
                    }
            }

            return null;
        }

        private void CreateRoom(List<List<TileContainer>> localArea)
        {
            rooms.Add(new Room(localArea));
        }

        public override string ToString()
        {
            var sb = new StringBuilder("area\n", tiles.Count * tiles.FirstOrDefault().Count);

            foreach (List<TileContainer> tileColumn in tiles)
            {
                foreach (TileContainer container in tileColumn)
                {
                    sb.Append(container.tile.rawValue);
                }
                sb.Append("\n");
            }

            return sb.ToString();
        }
    }


    public class Room
    {
        List<List<TileContainer>> tiles;

        public Room(List<List<TileContainer>> tiles)
        {
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
        }
    }


    public enum Direction
    {
        Column,
        Row
    }

    public static class AreaExtension
    {
        public static int GetArea<T>(this List<List<T>> tiles)
        {
            return tiles.Count * tiles.FirstOrDefault().Count;
        }
    }

    public static class DirectionExtension
    {
        public static Direction Reverse(this Direction direction)
        {
            if (direction == Direction.Column)
            {
                return Direction.Row;
            }
            else
            {
                return Direction.Column;
            }
        }
    }
}