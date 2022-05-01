using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using WorldMap;

#nullable enable
public class RooomTest
{
    Vector2 offset = new Vector2(30, 30);
    int columns = 10;
    int rows = 20;
#pragma warning disable CS8618
    List<List<TileContainer>> tiles;
    Room room;
    Room other;
#pragma warning restore CS8618
    [SetUp]
    public void SetUp()
    {
        tiles = Enumerable.Range(0, columns).Select(x =>
        {
            return Enumerable.Range(0, rows).Select(x => new TileContainer(new Floor())).ToList();
        }).ToList();
        room = new Room(tiles, offset);
        other = new Room(new List<List<TileContainer>>(tiles), null);
    }

    [Test]
    public void checkCenter()
    {
        Assert.AreEqual(offset + new Vector2(columns / 2, rows / 2), room.center);
    }

    [Test]
    public void CheckSize()
    {
        Assert.AreEqual(columns * rows, room.size);
    }

    [Test]
    public void CheckDistance()
    {
        Assert.AreEqual(offset.magnitude, room.Distance(other));
    }

    [Test]
    public void CheckCenterToCenter()
    {
        Assert.AreEqual(new Rect(columns / 2, rows / 2, offset.x, offset.y), room.CenterToCenter(other));
    }

    [Test]
    public void CheckGetPosition()
    {
        Assert.AreEqual(new Vector2(0, 0), room.GetPosition(tiles[0][0]));
    }

    [Test]
    public void CheckGetTerrainVertically()
    {
        var result = room.GetTerrain(tiles[0][1]);
        if (result == null)
        {
            Assert.Fail();
            return;
        }
        Assert.AreEqual(tiles[0], result);
    }

    [Test]
    public void CheckGetTerrainHorizontally()
    {
        var result = room.GetTerrain(tiles[1][0]);
        if (result == null)
        {
            Assert.Fail();
            return;
        }
        var terrain = new List<TileContainer>();
        foreach (int x in Enumerable.Range(0, tiles.Count))
        {
            terrain.Add(tiles[x][0]);
        }
        Assert.AreEqual(terrain, result);
    }
}
