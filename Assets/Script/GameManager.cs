using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameObject player;

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
        player = Instantiate(Resources.Load("Characters/Themisto"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
    }

    public void onDrag(Vector2 direction)
    {
        player.GetComponent<Player>().AttemptMove(direction);
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
