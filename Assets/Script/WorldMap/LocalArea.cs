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
        public int numberOfRoom = 1;


        public List<List<TileContainer>> tiles { get; private set; }
        private int maximumNumberOfRoom;
        private List<List<List<TileContainer>>> queue = new List<List<List<TileContainer>>>();

        public LocalArea(int columns, int rows, int maximumNumberOfRoom = 100, int minimumRoomSize = 0)
        {
            this.maximumNumberOfRoom = maximumNumberOfRoom;
            tiles = Enumerable.Range(0, columns).Select(x =>
            {
                return Enumerable.Range(0, rows).Select(x => new TileContainer(new Empty())).ToList();
            }).ToList();

            queue.Add(tiles);
            TryToCreateRooms(minimumRoomSize, Direction.Column);
        }

        private void TryToCreateRooms(int minimumRoomSize, Direction direction)
        {
            var dir = direction;
            while (queue.Count > 0)
            {
                // 部屋の候補の数が最大数に達している場合はこれ以上分割しない
                if (queue.Count >= maximumNumberOfRoom)
                {
                    break;
                }
                var area = queue.PopLargest();
                if (area == null)
                {
                    return;
                }

                var (area1, area2) = SplitArea(area, dir) ?? default;

                // 分割に失敗したか、分割した結果最小の面積を下回った場合は分割せず部屋を作成する
                // TODO: 最小の面積を下回った場合、分割位置を変えることで分割できないか確かめる
                if (area1 == null && area2 == null ||
                    area1?.GetSize() <= minimumRoomSize || area2?.GetSize() <= minimumRoomSize)
                {
                    CreateRoom(area);
                    continue;
                }

                if (area1 != null)
                {
                    queue.Add(area1);
                }
                if (area2 != null)
                {
                    queue.Add(area2);
                }
                dir = dir.Reverse();
            }

            foreach (int i in Enumerable.Range(0, queue.Count))
            {
                CreateRoom(queue[i]);
            }
            queue.RemoveAll((List<List<TileContainer>> _) => true);
        }

        private (List<List<TileContainer>>, List<List<TileContainer>>)? SplitArea(List<List<TileContainer>> map, Direction direction)
        {
            var minimumEdge = 7;
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
            if (numberOfRoom > maximumNumberOfRoom)
            {
                return;
            }
            numberOfRoom++;
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


    public enum Direction
    {
        Column,
        Row
    }

    public static class AreaExtension
    {
        public static int GetSize<T>(this List<List<T>> tiles)
        {
            return tiles.Count * tiles.FirstOrDefault().Count;
        }
    }

    public static class AreaQueueExtension
    {
        public static List<List<T>>? PopLargest<T>(this List<List<List<T>>> queue)
        {
            if (queue.Count < 1)
            {
                return null;
            }
            var tmp = queue[0];
            foreach (int i in Enumerable.Range(0, queue.Count))
            {
                if (tmp.GetSize() < queue[i].GetSize())
                {
                    tmp = queue[i];
                }
            }
            queue.Remove(tmp);
            return tmp;
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