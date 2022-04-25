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
                return new Vector2(tiles.Count / 2, tiles[0].Count / 2);
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

        public void MakeGate()
        {

        }
    }
}