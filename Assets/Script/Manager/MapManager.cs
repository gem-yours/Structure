using System.Collections;
using System.Collections.Generic;
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
    public int centerSize { get; } = 10;
    public int areaSize { get; } = 75;
#pragma warning disable CS8618
    public Ground overall;
#pragma warning restore CS8618

    private List<LocalArea> localAreas = new List<LocalArea>();

    void Start()
    {
        overall = new Ground(-centerSize + areaSize * 2, -centerSize + areaSize * 2);

        GenerateMap(centerSize, areaSize, 10);
        StartCoroutine(DetectWhereThePlayerIs(centerSize, areaSize));
        UIManager.instance.miniMap.ground = overall;
        StartCoroutine(DrawMap());
    }


    private void GenerateMap(int centerSize, int areaSize, int maxNumberOfRoom)
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
            UIManager.instance.miniMap.DrawMap(GameManager.instance.player.transform.position);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
