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

    private const float distanceThreshold = 10;
    private const float deadDistance = 25;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {

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

    private IEnumerator ContinuouslySpawn()
    {
        for (; ; )
        {
            AttemptSpawn();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void AttemptSpawn()
    {
        var currentRoom = MapManager.instance.currentRoom;
        if (currentRoom == null)
        {
            return;
        }
        if (MapManager.instance.currentArea == null)
        {
            return;
        }

        var positionInRoom = GenerateRandomPosition(currentRoom);
        if (positionInRoom is null) return;

        var location = (Vector2)positionInRoom + currentRoom.offset + MapManager.instance.currentArea.offset;
        // プレイヤーの近くに敵が出現しないようにする
        if (((Vector2)GameManager.instance.player.transform.position - location).magnitude < distanceThreshold)
        {
            return;
        }

        Spawn(location);
    }

    private Vector2? GenerateRandomPosition(WorldMap.Room room)
    {
        // 壁の中などに出現する場合再抽選を10回まで行う
        foreach (int numberOfRegenerate in Enumerable.Range(0, 10))
        {
            var position = new Vector2(Random.Range(0, room.ground.columns), Random.Range(0, room.ground.rows));
            if (room.ground.Get(position)?.tile.canPassThrough ?? false) return position;
        }
        return null;
    }

    private Enemy? Spawn(Vector3 location)
    {
        if (_enemies.Count > enemiesLimit)
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
            GameManager.instance.player.expManager.GainExp(enemy.exp);
        };
        enemy.target = GameManager.instance.player.gameObject;

        return enemyObj.GetComponent<Enemy>();
    }


    private IEnumerator KillEnemiesFarAwayPlayer()
    {
        for (; ; )
        {
            var room = MapManager.instance.currentRoom;
            var area = MapManager.instance.currentArea;
            var offset = (area is null) ? Vector2.zero : area.offset;
            Rect? rect = (room is null) ? null : room.rect;

            var player = GameManager.instance.player.transform.position;

            foreach (var enemy in enemies)
            {
                // 同じ部屋にいる敵は消さない
                if (rect is not null && ((Rect)rect).Contains((Vector2)enemy.transform.position - offset)) continue;
                if ((player - enemy.transform.position).magnitude > deadDistance)
                {
                    Dead(enemy);
                }
            }
            yield return new WaitForSeconds(3);
        }
    }

    public void KillAllEnemies()
    {
        while (enemies.Count > 0)
        {
            var enemy = enemies.FirstOrDefault();
            Dead(enemy);
        }
    }

    private void Start()
    {
        StartCoroutine(ContinuouslySpawn());
        StartCoroutine(KillEnemiesFarAwayPlayer());
    }
}
