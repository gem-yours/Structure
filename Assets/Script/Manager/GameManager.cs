using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        MapGenerator.Generate(Vector2.zero, 25, 10, Resources.Load("Map/Tile") as GameObject, Resources.Load("Map/Wall") as GameObject);

        playerObject = Instantiate(Resources.Load("Characters/Themisto"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        player = playerObject.GetComponent<Player>();

        gameObject.AddComponent<GameCamera>();
        gameCamera = gameObject.GetComponent<GameCamera>();
        gameCamera.target = playerObject;

        UIManager.instance.onCast = (SpellSlot slot) => player.Cast(slot);

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
