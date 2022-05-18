using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager instance;
    public static int enemiesLimit = 5;

    public List<Enemy> enemies
    {
        get
        {
            return _enemies.Select(enemy =>
            {
                return enemy.GetComponent<Enemy>();
            }).ToList();
        }
    }
    private List<GameObject> _enemies = new List<GameObject>();

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

    private void Init()
    {

    }


    private IEnumerator AttemptSpawn()
    {
        for (; ; )
        {
            var room = MapManager.instance.currentRoom;
            if (room == null)
            {
                yield return new WaitForSeconds(1);
                continue;
            }
            var rect = room.rect;
            if (rect == null)
            {
                yield return new WaitForSeconds(1);
                continue;
            }
            if (MapManager.instance.currentArea == null)
            {
                yield return new WaitForSeconds(1);
                continue;
            }
            var offset = MapManager.instance.currentArea.offset;

            var position = new Vector2(Random.Range(rect.x, rect.x + rect.width), Random.Range(rect.y, rect.y + rect.height));
            Spawn(position + offset);
            yield return new WaitForSeconds(1);
        }
    }

    private GameObject Spawn(Vector3 location)
    {
        if (_enemies.Count > enemiesLimit)
        {
            return null;
        }

        // プレイヤーの近くに敵が出現しないようにする
        if ((GameManager.instance.player.transform.position - location).magnitude < 10)
        {
            return null;
        }

        var enemyObj = Instantiate(Resources.Load("Enemies/FireElement"), location, Quaternion.identity) as GameObject;
        _enemies.Add(enemyObj);
        var enemy = enemyObj.GetComponent<Enemy>();
        enemy.onDead = (Enemy enemy) =>
        {
            Dead(enemy);
        };
        enemy.target = GameManager.instance.player.gameObject;


        return enemyObj;
    }

    public bool Dead(Enemy enemy)
    {
        var enemyObject = _enemies.Find(
            delegate (GameObject go)
            {
                return go.GetComponent<Enemy>() == enemy;
            }
        );
        if (enemyObject == null)
        {
            return false;
        }

        GameManager.instance.player.expManager.GainExp(enemy.exp);

        _enemies.Remove(enemyObject);
        Destroy(enemyObject);
        return true;
    }

    public GameObject NearestEnemy(Vector3 position)
    {
        if (_enemies.Count == 0)
        {
            return null;
        }
        _enemies.Sort(delegate (GameObject lhs, GameObject rhs)
        {
            var lhsDistance = (position - lhs.transform.position).magnitude;
            var rhsDistance = (position - rhs.transform.position).magnitude;
            return (lhsDistance > rhsDistance) ? 1 : -1;
        });
        return _enemies[0];
    }

    private void Start()
    {
        StartCoroutine(AttemptSpawn());
    }
}
