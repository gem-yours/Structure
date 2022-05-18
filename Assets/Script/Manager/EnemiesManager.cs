using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
public class EnemiesManager : MonoBehaviour
{
#pragma warning disable CS8618
    public static EnemiesManager instance;
#pragma warning restore CS8618
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

    private int distanceThreshold = 10;

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

    private GameObject? Spawn(Vector3 location)
    {
        if (_enemies.Count > enemiesLimit)
        {
            return null;
        }

        // プレイヤーの近くに敵が出現しないようにする
        if ((GameManager.instance.player.transform.position - location).magnitude < distanceThreshold)
        {
            return null;
        }

        var enemyObj = Instantiate(Resources.Load("Enemies/FireElement"), location, Quaternion.identity) as GameObject;
        if (enemyObj == null) return null;
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

    public GameObject? NearestEnemy(Vector3 position)
    {
        if (_enemies.Count == 0)
        {
            return null;
        }
        _enemies.Sort((GameObject lhs, GameObject rhs) =>
        {
            var lhsDistance = (position - lhs.transform.position).magnitude;
            var rhsDistance = (position - rhs.transform.position).magnitude;
            return (lhsDistance > rhsDistance) ? 1 : -1;
        });
        var enemy = _enemies.FirstOrDefault();
        if ((position - enemy.transform.position).magnitude < distanceThreshold)
        {
            return enemy;
        }
        else
        {
            return null;
        }
    }

    private void Start()
    {
        StartCoroutine(AttemptSpawn());
    }
}
