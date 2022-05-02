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


        public Ground ground { get; private set; }
        private int maximumNumberOfRoom;
        private List<Ground> queue = new List<Ground>();

        public LocalArea(Vector2 size, int maximumNumberOfRoom = 100, int minimumRoomSize = 0) : this((int)size.x, (int)size.y, maximumNumberOfRoom, minimumRoomSize) { }

        public LocalArea(int columns, int rows, int maximumNumberOfRoom = 100, int minimumRoomSize = 0)
        {
            this.maximumNumberOfRoom = maximumNumberOfRoom;
            ground = new Ground(
                Enumerable.Range(0, columns).Select(x =>
                {
                    return Enumerable.Range(0, rows).Select(x => new TileContainer(new Empty())).ToList();
                }).ToList()
            );

            queue.Add(ground);
            TryToCreateRooms(minimumRoomSize, Direction.Column);
        }

        private void TryToCreateRooms(int minimumRoomSize, Direction direction)
        {
            var dir = direction;
            while (queue.Count > 0)
            {
                // 部屋の候補の数が最大数に達している場合はこれ以上分割しない
                if (numberOfRoom + queue.Count - Random.Range(0, maximumNumberOfRoom / 2) > maximumNumberOfRoom)
                {
                    break;
                }
                var ground = queue.PopLargest();
                if (ground == null)
                {
                    return;
                }

                var (area1, area2) = ground.SplitArea(dir) ?? default;

                // 分割に失敗したか、分割した結果最小の面積を下回った場合は分割せず部屋を作成する
                // TODO: 最小の面積を下回った場合、分割位置を変えることで分割できないか確かめる
                if (area1 == null && area2 == null ||
                    area1?.size <= minimumRoomSize || area2?.size <= minimumRoomSize)
                {
                    CreateRoom(ground.tiles);
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
                CreateRoom(queue[i].tiles);
            }
            queue.RemoveAll((Ground _) => true);
        }


        private void CreateRoom(List<List<TileContainer>> localArea)
        {
            if (numberOfRoom > maximumNumberOfRoom)
            {
                return;
            }
            numberOfRoom++;
            rooms.Add(new Room(localArea, ground.GetOffset(localArea[0][0])));
        }


        public static bool IsAdjacent(Room room, Room target, List<List<TileContainer>> tiles, List<Room> rooms)
        {
            var rect = room.CenterToCenter(target);

            // 部屋の中心を対角線とする四角形のなかに空白タイルや他の部屋がなければ隣接している
            foreach (int x in Enumerable.Range((int)rect.xMin, Mathf.Max((int)rect.xMax - (int)rect.xMin, 1)))
            {
                foreach (int y in Enumerable.Range((int)rect.yMin, Mathf.Max((int)rect.yMax - (int)rect.yMin, 1)))
                {
                    if (tiles[x][y].tile.rawValue == new Empty().rawValue)
                    {
                        return false;
                    }
                }
            }

            foreach (Room r in rooms)
            {
                if (r == room || r == target)
                {
                    continue;
                }
                if (rect.Contains(r.center))
                {
                    return false;
                }
            }

            return true;
        }


        public override string ToString()
        {
            var sb = new StringBuilder("area\n", ground.size);

            foreach (List<TileContainer> tileColumn in ground.tiles)
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


    public static class AreaExtension
    {
        public static int GetSize<T>(this List<List<T>> tiles)
        {
            return tiles.Count * tiles.FirstOrDefault().Count;
        }

        public static Vector2 GetOffset<T>(this List<List<T>> tiles, T target) where T : System.IEquatable<T>
        {
            foreach (int x in Enumerable.Range(0, tiles.Count))
            {
                foreach (int y in Enumerable.Range(0, tiles[x].Count))
                {
                    if (tiles[x][y].Equals(target))
                    {
                        return new Vector2(x, y);
                    }
                }
            }
            return Vector2.zero;
        }
    }

    public static class AreaQueueExtension
    {
        public static Ground? PopLargest(this List<Ground> queue)
        {
            if (queue.Count < 1)
            {
                return null;
            }
            var tmp = queue[0];
            foreach (int i in Enumerable.Range(0, queue.Count))
            {
                if (tmp.size < queue[i].size)
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

    public static class RoomsExtension
    {
        public static List<Room> CloseRooms(this List<Room> rooms, Room room, int size)
        {
            var tmp = new List<Room>(rooms);
            tmp.Sort((Room lhs, Room rhs) =>
            {
                return (lhs.Distance(room) > rhs.Distance(room)) ? 1 : -1;
            });

            tmp.RemoveRange(size, tmp.Count - size - 1);
            return tmp;
        }
    }
}