using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace WorldMap
{
    public class TileContainer : System.IEquatable<TileContainer>, System.IEquatable<Tile>
    {
        public Tile tile;

        public TileContainer(Tile tile)
        {
            this.tile = tile;
        }

        public Object? Resource()
        {
            return Resources.Load("Map/" + tile.resourcePath);
        }

        public bool Equals(TileContainer other)
        {
            return tile.rawValue == other.tile.rawValue;
        }

        public bool Equals(Tile other)
        {
            return tile.rawValue == other.rawValue;
        }
    }
    public interface Tile
    {
        string rawValue { get; }
        string resourcePath { get; }
    }

    public class Empty : Tile
    {
        public string rawValue { get; } = "*";
        public string resourcePath { get; } = "";
    }

    public class NorthWall : Tile
    {
        public string rawValue { get; } = "~";
        public string resourcePath { get; } = "Wall";
    }

    public class SouthWall : Tile
    {
        public string rawValue { get; } = "=";
        public string resourcePath { get; } = "Wall";
    }

    public class VerticalWall : Tile
    {
        public string rawValue { get; } = "+";
        public string resourcePath { get; } = "Wall";

    }

    public class Floor : Tile
    {
        public string rawValue { get; } = "#";
        public string resourcePath { get; } = "Tile";

    }
}