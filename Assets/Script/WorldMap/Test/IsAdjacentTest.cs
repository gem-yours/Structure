using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using WorldMap;


#nullable enable
public class IsAdjacentTest
{
#pragma warning disable CS8618
    List<List<TileContainer>> tiles;
#pragma warning restore CS8618

    [SetUp]
    public void Setup()
    {
        tiles = Enumerable.Range(0, 10).Select(x =>
        {
            return Enumerable.Range(0, 10).Select(x => new TileContainer(new Floor())).ToList();
        }).ToList();
    }


    [Test]
    public void CheckAdjacent()
    {
        var room1 = new Room(tiles.GetRange(0, 5), null);
        var room2 = new Room(tiles.GetRange(5, tiles.Count - 1 - 5), new Vector2(5, 0));
        Assert.True(LocalArea.IsAdjacent(room1, room2, tiles, new List<Room>()));
    }

    [Test]
    public void CheckAdjacentWithEmptyBetWeenRooms()
    {
        var room1 = new Room(tiles.GetRange(0, 2), null);
        var room2 = new Room(tiles.GetRange(6, tiles.Count - 1 - 6), new Vector2(6, 0));

        foreach (TileContainer tileContainer in tiles[4])
        {
            tileContainer.tile = new Empty();
        }

        Assert.False(LocalArea.IsAdjacent(room1, room2, tiles, new List<Room>()));
    }

    [Test]
    public void CheckAdjacentWithAnotherRoomBetWeenRooms()
    {
        var room1 = new Room(tiles.GetRange(0, 2), null);
        var room2 = new Room(tiles.GetRange(3, 1), new Vector2(3, 0));
        var room3 = new Room(tiles.GetRange(5, tiles.Count - 1 - 5), new Vector2(5, 0));

        Assert.False(LocalArea.IsAdjacent(room1, room3, tiles, new List<Room> { room1, room2, room3 }));
        Assert.True(LocalArea.IsAdjacent(room1, room2, tiles, new List<Room> { room1, room2, room3 }));
        Assert.True(LocalArea.IsAdjacent(room2, room3, tiles, new List<Room> { room1, room2, room3 }));
    }

    [Test]
    public void CheckCenter()
    {
        var room1 = new Room(tiles.GetRange(0, 5), null);
        var room2 = new Room(tiles.GetRange(5, tiles.Count - 1 - 5), new Vector2(5, 0));

        var rect = new Rect(
                Mathf.Min(room1.center.x, room2.center.x),
                Mathf.Min(room1.center.y, room2.center.y),
                Mathf.Abs(room1.center.x - room2.center.x),
                Mathf.Abs(room1.center.y - room2.center.y)
            );
        Assert.AreEqual(5, rect.width);
        Assert.AreEqual(0, rect.height);
    }
}
