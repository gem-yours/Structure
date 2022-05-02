using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;

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
        GenerateMap(10, 100, 10);

        playerObject = Instantiate(Resources.Load("Characters/Themisto"), Vector3.zero, Quaternion.identity) as GameObject;
        player = playerObject.GetComponent<Player>();
        UIManager.instance.deck = player.deck;

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

    private void GenerateMap(int centerSize, int mapSize, int maxNumberOfRoom)
    {
        var center = new Vector2(centerSize, centerSize);
        var map = new Vector2(mapSize, mapSize - centerSize);

        // 中心
        WorldMap.Generator.Generate(
            new WorldMap.LocalArea(center, 1),
            Vector2.Scale(-center, new Vector2(0.5f, 0.5f))
        );

        // 左下
        WorldMap.Generator.Generate(
            new WorldMap.LocalArea(map, (int)(Random.value * maxNumberOfRoom)),
            new Vector2(-map.x + center.x / 2, -map.y - center.y / 2)
        );
        // 左上
        WorldMap.Generator.Generate(
            new WorldMap.LocalArea(map.Swap(), (int)(Random.value * maxNumberOfRoom)),
            new Vector2(-map.x + center.x / 2, -center.y / 2)
        );
        // 右上
        WorldMap.Generator.Generate(
            new WorldMap.LocalArea(map, (int)(Random.value * maxNumberOfRoom)),
            new Vector2(-center.x / 2, center.y / 2)
        );
        // 右下
        WorldMap.Generator.Generate(
            new WorldMap.LocalArea(map.Swap(), (int)(Random.value * maxNumberOfRoom)),
            new Vector2(center.x / 2, -map.y - center.y / 2)
        );

    }
}

static class VectorExtension
{
    public static Vector2 Swap(this Vector2 vector)
    {
        return new Vector2(vector.y, vector.x);
    }
}
