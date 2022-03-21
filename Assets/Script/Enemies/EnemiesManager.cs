using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager instance;
    public static int enemiesLimit = 10;
    private List<GameObject> enemies = new List<GameObject>();

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
        
    }

    public GameObject Spawn(Vector3 location)
    {
        if(enemies.Count > enemiesLimit) {
            return null;
        }
        var enemy = Instantiate(Resources.Load("Enemies/FireElement"), location, Quaternion.identity) as GameObject;
        enemies.Add(enemy);
        return enemy;
    }

    public GameObject NearestEnemy()
    {
        if (enemies.Count == 0)
        {
            return null;
        }
        // TODO: 距離を検索する
        return enemies[0];
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
