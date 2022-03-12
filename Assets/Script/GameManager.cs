using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameObject playerObject;
    private Player player;

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
    }

    public void onDrag(Vector2 direction)
    {
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
