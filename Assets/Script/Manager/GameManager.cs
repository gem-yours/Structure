using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;

    private GameObject playerObject;

    private GameCamera gameCamera;

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
        MapGenerator.Generate(Vector2.zero, 25, 10, Resources.Load("Map/Tile") as GameObject, Resources.Load("Map/Wall") as GameObject);

        playerObject = Instantiate(Resources.Load("Characters/Themisto"), Vector3.zero, Quaternion.identity) as GameObject;
        player = playerObject.GetComponent<Player>();
        UIManager.instance.deck = player.deck;

        player.expManager.onLevelUp = (int level) =>
        {
            Pause();
            UIManager.instance.ShowPickSpellWindow(
                new FireBolt(),
                new FireBolt(),
                new FireBolt(),
                (Spell spell) =>
                {
                    if (spell != null)
                    {
                        player.deck.AddSpell(spell);
                    }
                    UIManager.instance.HidePickSpellWindow();
                    Resume();
                }
            );
        };

        gameObject.AddComponent<GameCamera>();
        gameCamera = gameObject.GetComponent<GameCamera>();
        gameCamera.target = playerObject;

        UIManager.instance.onCast = (SpellSlot slot) => player.Attack(slot);

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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
