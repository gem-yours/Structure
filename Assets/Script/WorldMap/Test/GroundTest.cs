using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using WorldMap;
public class GroundTest
{
#pragma warning disable CS8618
    Ground ground;
#pragma warning restore CS8618

    [SetUp]
    public void SetUp()
    {
        ground = new Ground(50, 50);
    }

    [Test]
    public void AddGround()
    {
        var target = new Ground(25, 25);
        var position = new Vector2(10, 10);
        target.Get(position).tile = new VerticalWall();
        ground.Add(target, Vector2.zero);
        Assert.AreEqual(
            ground.Get(position),
            target.Get(position)
        );
    }

    [Test]
    public void AddGroundWithOffset()
    {
        var target = new Ground(25, 25);
        var position = new Vector2(10, 10);
        var offset = new Vector2(5, 5);
        target.Get(position).tile = new VerticalWall();
        ground.Add(target, offset);
        Assert.AreEqual(
            ground.Get(position + offset),
            target.Get(position)
        );
    }
}
