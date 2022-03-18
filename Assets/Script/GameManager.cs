using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Rigidbody2D playerRb2D;

    private GameObject playerObject;
    private Player player;

    private GameCamera gameCamera;

    private Button attackButton;
    private Button spell1Button;
    private Button spell2Button;
    private Button spell3Button;
    private Button uniqueButton;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance == this)
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        playerObject = Instantiate(Resources.Load("Characters/Themisto"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        player = playerObject.GetComponent<Player>();
        playerRb2D = playerObject.GetComponent<Rigidbody2D>();

        gameObject.AddComponent<GameCamera>();
        gameCamera = gameObject.GetComponent<GameCamera>();
        gameCamera.target = playerObject;

        attackButton = GameObject.Find("AttackButton").GetComponent<Button>();
        attackButton.onClick.AddListener(() => player.Cast(SpellSlot.Attack));
        spell1Button = GameObject.Find("Spell1Button").GetComponent<Button>();
        spell1Button.onClick.AddListener(() => player.Cast(SpellSlot.Spell1));
        spell2Button = GameObject.Find("Spell2Button").GetComponent<Button>();
        spell2Button.onClick.AddListener(() => player.Cast(SpellSlot.Spell2));
        spell3Button = GameObject.Find("Spell3Button").GetComponent<Button>();
        spell3Button.onClick.AddListener(() => player.Cast(SpellSlot.Spell3));
        uniqueButton = GameObject.Find("UniqueButton").GetComponent<Button>();
        uniqueButton.onClick.AddListener(() => player.Cast(SpellSlot.Unique));

        MapGenerator.Generate(Vector2.zero, 100, 50, Resources.Load("Map/Glass") as GameObject);
    }

    public void onDrag(Vector2 direction)
    {
        // y方向のセンシを下げる
        var correctedDir = Vector2.Scale(direction, new Vector2(1, 0.5f));
        player.ChangeMoveDirection(correctedDir);
    }

    public void EndDragging()
    {
        player.ChangeMoveDirection(Vector2.zero);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
