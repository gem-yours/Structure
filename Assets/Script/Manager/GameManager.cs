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
        UIManager.instance.loadingText.text = "プレイヤーを作成中";

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

        UIManager.instance.menuButton.onClick.AddListener(() =>
        {
            Pause();
            UIManager.instance.ShowMenu(() =>
            {
                UIManager.instance.HideMenu();
                Resume();
            });
            MapManager.instance.Draw(UIManager.instance.worldMap, player.transform.position, null);
        });

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

        player.onCasting = (Spell spell, float current) =>
        {
            var slot = player.deck.GetSpellSlot(spell);
            if (slot is null) return;
            UIManager.instance.GetControllerBySlot((SpellSlot)slot).image.fillAmount = current;
        };
        UIManager.instance.maxHp = player.maxHp;
        player.onDamaged = (float hp) =>
        {
            UIManager.instance.currentHp = hp;
        };

        gameObject.AddComponent<GameCamera>();
        gameCamera = gameObject.GetComponent<GameCamera>();
        gameCamera.target = playerObject;

        UIManager.instance.movingController.onDragging = (Vector2 displacement) =>
        {
            // y方向のセンシを下げる
            var correctedDir = Vector2.Scale(displacement, new Vector2(1, 0.5f));
            player.ChangeMoveDirection(correctedDir);
        };
        UIManager.instance.movingController.onEndDragging = () =>
        {
            player.ChangeMoveDirection(Vector2.zero);
        };

        UIManager.instance.onDraggingSpell = (SpellSlot slot, Vector2 displacement) =>
        {
            player.Dragged(slot, displacement);
        };
        UIManager.instance.onClickSpell = (SpellSlot slot) =>
        {
            player.Clicked(slot);
        };
        UIManager.instance.onEndDragging = (SpellSlot slot) =>
        {
            player.EndDragging(slot);
        };
        UIManager.instance.onSpellPushed = (SpellSlot slot) =>
        {
            player.Pushed(slot);
        };
        UIManager.instance.onAttack = () =>
        {
            player.Attack();
        };

        UIManager.instance.loadingText.text = "マップを生成中";
        MapManager.instance.GenerateMap();
        UIManager.instance.loadingScreen.SetActive(false);
    }
}
