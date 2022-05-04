using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMap;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public Room currentRoom = null;

    public int centerSize { get; private set; } = 10;
    public int areaSize { get; private set; } = 75;

    private List<LocalArea> localAreas = new List<LocalArea>();
    private GameObject playerObject;

    public GameCamera gameCamera;

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

    public void Pause()
    {
        Time.timeScale = 0;
        UIManager.instance.isUiActive = false;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        UIManager.instance.isUiActive = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerObject = Instantiate(Resources.Load("Characters/Themisto"), Vector3.zero, Quaternion.identity) as GameObject;
        player = playerObject.GetComponent<Player>();
        UIManager.instance.deck = player.deck;

        GenerateMap(centerSize, areaSize, 10);
        StartCoroutine(DetectWhereThePlayerIs(centerSize, areaSize));

        player.expManager.onLevelUp = (int level) =>
        {
            Pause();
            UIManager.instance.ShowPickSpellWindow(
                new Explosion(),
                new Ignis(),
                new Ignis(),
                (Spell spell) =>
                {
                    if (spell != null)
                    {
                        player.deck.Add(spell);
                    }
                    UIManager.instance.HidePickSpellWindow();
                    Resume();
                }
            );
        };

        player.expManager.onExpGain = (int level, int exp, int requireExp) =>
        {
            UIManager.instance.level = level;
            UIManager.instance.requireExp = requireExp;
            UIManager.instance.exp = exp;
        };

        player.nearestEnemy = (Vector2 location) =>
        {
            return EnemiesManager.instance.NearestEnemy(location);
        };

        gameObject.AddComponent<GameCamera>();
        gameCamera = gameObject.GetComponent<GameCamera>();
        gameCamera.target = playerObject;

        UIManager.instance.dragController.onDragging = (Vector2 displacement) =>
        {
            // y方向のセンシを下げる
            var correctedDir = Vector2.Scale(displacement, new Vector2(1, 0.5f));
            player.ChangeMoveDirection(correctedDir);
        };
        UIManager.instance.dragController.onEndDragging = () =>
        {
            player.ChangeMoveDirection(Vector2.zero);
        };

        UIManager.instance.onDragging = (SpellSlot slot, Vector2 displacement) =>
        {
            player.IndicateDirection(displacement);
        };
        UIManager.instance.onEndDragging = (SpellSlot slot) =>
        {
            player.Cast(slot);
        };
        UIManager.instance.onAttack = () =>
        {
            player.Attack();
        };
    }

    private void GenerateMap(int centerSize, int areaSize, int maxNumberOfRoom)
    {
        var center = new Vector2(centerSize, centerSize);
        var map = new Vector2(areaSize, areaSize - centerSize);

        var centerRoom = new WorldMap.LocalArea(center, 1);
        WorldMap.Generator.Generate(
            centerRoom,
            Vector2.Scale(-center, new Vector2(0.5f, 0.5f))
        );
        localAreas.Add(centerRoom);

        var bottomLeading = new WorldMap.LocalArea(map, (int)Random.Range(maxNumberOfRoom / 2, maxNumberOfRoom));
        WorldMap.Generator.Generate(
            bottomLeading,
            new Vector2(-map.x + center.x / 2, -map.y - center.y / 2)
        );
        localAreas.Add(bottomLeading);

        var topLeading = new WorldMap.LocalArea(map.Swap(), (int)Random.Range(maxNumberOfRoom / 2, maxNumberOfRoom));
        WorldMap.Generator.Generate(
            topLeading,
            new Vector2(-map.x + center.x / 2, -center.y / 2)
        );
        localAreas.Add(topLeading);

        var topTrailing = new WorldMap.LocalArea(map, (int)Random.Range(maxNumberOfRoom / 2, maxNumberOfRoom));
        WorldMap.Generator.Generate(
            topTrailing,
            new Vector2(-center.x / 2, center.y / 2)
        );
        localAreas.Add(topTrailing);

        var bottomTrailing = new WorldMap.LocalArea(map.Swap(), (int)Random.Range(maxNumberOfRoom / 2, maxNumberOfRoom));
        WorldMap.Generator.Generate(
           bottomTrailing,
            new Vector2(center.x / 2, -map.y - center.y / 2)
        );
        localAreas.Add(bottomTrailing);
    }

    private IEnumerator DetectWhereThePlayerIs(int centerSize, int areaSize, int timeInterval = 1)
    {
        // プレイヤーのいちは中心が0の座標系だが、LocalAreaは左下が0なので補正する
        var offsetValue = (areaSize + centerSize / 2f) / 2f;
        var offset = new Vector2(offsetValue, offsetValue);
        for (; ; )
        {
            if (player != null)
            {
                foreach (LocalArea localArea in localAreas)
                {
                    currentRoom = localArea.GetRoom((Vector2)player.gameObject.transform.position + offset);
                    if (currentRoom != null)
                    {
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(timeInterval);
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
