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
        private List<List<TileContainer>> tiles;
        List<Room> rooms = new List<Room>();

        public LocalArea(int columns, int rows)
        {
            tiles = Enumerable.Range(0, columns).Select(x =>
            {
                return Enumerable.Range(0, rows).Select(x => new TileContainer(new Empty())).ToList();
            }).ToList();
            TryToCreateRooms(tiles, Mathf.Max(columns, rows) * 3);
        }

        private void TryToCreateRooms(List<List<TileContainer>> localArea, int minimumArea)
        {
            if (localArea.GetArea() < minimumArea)
            {
                CreateRoom(localArea);
                return;
            }
            var (area1, area2) = SplitArea(localArea);
            if (area1 == null && area2 == null)
            {
                CreateRoom(localArea);
                return;
            }
            if (area1 != null) TryToCreateRooms(area1, minimumArea);
            if (area2 != null) TryToCreateRooms(area2, minimumArea);
        }

        private (List<List<TileContainer>>?, List<List<TileContainer>>?) SplitArea(List<List<TileContainer>> map)
        {
            if (map.Count > 6)
            {
                // 3行は残しておかないと壁しかない部屋ができてしまう
                var border = Random.Range(3, map.Count - 1 - 3);
                var first = map.GetRange(0, border);
                var second = map.GetRange(border, map.Count - border);
                return (first, second);
            }

            var row = map.FirstOrDefault();
            if (row.Count > 6)
            {
                var border = Random.Range(3, row.Count - 1 - 3);
                var first = new List<List<TileContainer>>();
                var second = new List<List<TileContainer>>();
                for (int i = 0; i < map.Count; i++)
                {
                    first.Add(map[i].GetRange(0, border));
                    second.Add(map[i].GetRange(border, row.Count - border));
                }
                return (first, second);
            }

            return (null, null);
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

    public static class AreaExtension
    {
        public static int GetArea<T>(this List<List<T>> tiles)
        {
            return tiles.Count * tiles.FirstOrDefault().Count;
        }
    }
}