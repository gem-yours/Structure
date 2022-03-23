using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager instance;
    public static int enemiesLimit = 5;
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

        // プレイヤーの近くに敵が出現しないようにする
        if((GameManager.instance.player.position - location).magnitude < 10)
        {
            return null;
        }

        var enemy = Instantiate(Resources.Load("Enemies/FireElement"), location, Quaternion.identity) as GameObject;
        enemies.Add(enemy);
        return enemy;
    }

    public bool Dead(Enemy enemy)
    {
        var enemyObject = enemies.Find(
            delegate(GameObject go)
            {
                return go.GetComponent<Enemy>() == enemy;
            }
        );
        if (enemyObject == null)
        {
            return false;
        }

        GameManager.instance.player.expManager.GainExp(enemy.exp);

        enemies.Remove(enemyObject);
        Destroy(enemyObject);
        return true;
    }

    public GameObject NearestEnemy(Vector3 position)
    {
        if (enemies.Count == 0)
        {
            return null;
        }
        enemies.Sort(delegate(GameObject lhs, GameObject rhs){
            var lhsDistance = (position - lhs.transform.position).magnitude;
            var rhsDistance = (position - rhs.transform.position).magnitude;
            return (lhsDistance > rhsDistance) ? 1 : -1;
        });
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
