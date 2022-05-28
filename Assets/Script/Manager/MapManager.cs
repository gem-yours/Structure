using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldMap;


#nullable enable
public class MapManager : MonoBehaviour
{
#pragma warning disable CS8618
    public static MapManager instance;
#pragma warning restore CS8618
    public Room? currentRoom { get; private set; }
    public LocalArea? currentArea { get; private set; }
    public const int centerSize = 10;
    public const int areaSize = 75;
    public const int maxNumberOfRoom = 10;

    public Ground overall = new Ground(-centerSize + areaSize * 2, -centerSize + areaSize * 2);
    private List<LocalArea> localAreas = new List<LocalArea>();

    public void Draw(MiniMap miniMap, Vector2 playerPosition, Vector2? center)
    {
        if (center == null) center = Vector2.zero;
        miniMap.ground = overall;
        miniMap.resolution = overall.columns;
        StartCoroutine(_Draw(miniMap, playerPosition, (Vector2)center));
    }

    private IEnumerator _Draw(MiniMap miniMap, Vector2 playerPosition, Vector2 center)
    {
        yield return null;
        miniMap.DrawMap(center, playerPosition);
        miniMap.AddEnemy(EnemiesManager.instance.enemies.Select(x => (Vector2)x.transform.position).ToList());
        miniMap.Apply();
    }

    private void Start()
    {
        StartCoroutine(DetectWhereThePlayerIs(centerSize, areaSize));
        var miniMap = UIManager.instance.miniMap;
        miniMap.ground = overall;
        StartCoroutine(DrawMap());
    }


    public void GenerateMap()
    {
        var center = new Vector2(centerSize, centerSize);
        var map = new Vector2(areaSize, areaSize - centerSize);

        // 中心を左下にずらす
        var overallOffset = new Vector2(overall.columns, overall.rows) / 2;

        var centerRoom = new WorldMap.LocalArea(
            center,
            Vector2.Scale(-center, new Vector2(0.5f, 0.5f)),
            1
        );
        WorldMap.Generator.Generate(centerRoom);
        localAreas.Add(centerRoom);
        overall.Add(centerRoom, overallOffset);

        var bottomLeading = new WorldMap.LocalArea(
            map,
            new Vector2(-map.x + center.x / 2, -map.y - center.y / 2),
            (int)Random.Range(maxNumberOfRoom / 2, maxNumberOfRoom)
        );
        WorldMap.Generator.Generate(bottomLeading);
        localAreas.Add(bottomLeading);
        overall.Add(bottomLeading, overallOffset);

        var topLeading = new WorldMap.LocalArea(
            map.Swap(),
            new Vector2(-map.x + center.x / 2, -center.y / 2),
            (int)Random.Range(maxNumberOfRoom / 2, maxNumberOfRoom)
        );
        WorldMap.Generator.Generate(topLeading);
        localAreas.Add(topLeading);
        overall.Add(topLeading, overallOffset);

        var topTrailing = new WorldMap.LocalArea(
            map,
            new Vector2(-center.x / 2, center.y / 2), (int)Random.Range(maxNumberOfRoom / 2, maxNumberOfRoom)
        );
        WorldMap.Generator.Generate(topTrailing);
        localAreas.Add(topTrailing);
        overall.Add(topTrailing, overallOffset);

        var bottomTrailing = new WorldMap.LocalArea(
            map.Swap(),
            new Vector2(center.x / 2, -map.y - center.y / 2), (int)Random.Range(maxNumberOfRoom / 2, maxNumberOfRoom)
        );
        WorldMap.Generator.Generate(bottomTrailing);
        localAreas.Add(bottomTrailing);
        overall.Add(bottomTrailing, overallOffset);

        WorldMap.Generator.CreateOuterWall(new Rect(
            -areaSize + centerSize / 2 - 1,
            -areaSize + centerSize / 2 - 1,
            areaSize * 2 - centerSize + 1,
            areaSize * 2 - centerSize + 1
        ));
    }

    private IEnumerator DetectWhereThePlayerIs(int centerSize, int areaSize, int timeInterval = 1)
    {
        for (; ; )
        {
            var player = GameManager.instance.player;
            if (player != null)
            {
                foreach (LocalArea localArea in localAreas)
                {
                    if (!localArea.rect.Contains(player.gameObject.transform.position))
                    {
                        continue;
                    }
                    currentRoom = localArea.GetRoom((Vector2)player.gameObject.transform.position);
                    if (currentRoom != null)
                    {
                        currentArea = localArea;
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(timeInterval);
        }
    }


    private IEnumerator DrawMap()
    {
        for (; ; )
        {
            var position = GameManager.instance.player.transform.position;
            yield return _Draw(UIManager.instance.miniMap, position, position);
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}

static class VectorExtension
{
    public static Vector2 Swap(this Vector2 vector)
    {
        return new Vector2(vector.y, vector.x);
    }
}

static class GroundExtension
{
    public static void Add(this Ground ground, LocalArea localArea, Vector2 offset)
    {
        ground.Add(localArea.ground, localArea.offset + offset);
    }
}
