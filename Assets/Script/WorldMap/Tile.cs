using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace WorldMap
{
    public class TileContainer
    {
        public Tile tile;

        public TileContainer(Tile tile)
        {
            this.tile = tile;
        }
    }
    public interface Tile
    {
        string rawValue { get; }
    }

    public class Empty : Tile
    {
        public string rawValue { get; } = "*";
    }

    public class NorthWall : Tile
    {
        public string rawValue { get; } = "~";
    }

    public class SouthWall : Tile
    {
        public string rawValue { get; } = "=";
    }

    public class HorizontalWall : Tile
    {
        public string rawValue { get; } = "+";

    }

    public class Floor : Tile
    {
        public string rawValue { get; } = "#";

    }
}