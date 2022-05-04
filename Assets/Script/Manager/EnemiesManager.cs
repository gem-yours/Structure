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
        // プレイヤーのいちは中心が0の座標系だが、LocalAreaは左下が0なので補正する
        // TODO: 知識が漏れ出しているような感じがするのでLocalAreaやRoomの方でいい感じに処理してほしい
        var offsetValue = (GameManager.instance.areaSize + GameManager.instance.centerSize / 2f) / 2f;
        var offset = new Vector2(offsetValue, offsetValue);
        for (; ; )
        {
            var rect = GameManager.instance.currentRoom.rect;
            if (rect == null) continue;
            var position = new Vector2(Random.Range(rect.x, rect.x + rect.width), Random.Range(rect.y, rect.y + rect.height));
            Spawn(position - offset);
            yield return new WaitForSeconds(1);
        }
    }

    private GameObject Spawn(Vector3 location)
    {
        if (enemies.Count > enemiesLimit)
        {
            return null;
        }

        // プレイヤーの近くに敵が出現しないようにする
        if ((GameManager.instance.player.transform.position - location).magnitude < 10)
        {
            return null;
        }

        var enemyObj = Instantiate(Resources.Load("Enemies/FireElement"), location, Quaternion.identity) as GameObject;
        enemies.Add(enemyObj);
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
        var enemyObject = enemies.Find(
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
        enemies.Sort(delegate (GameObject lhs, GameObject rhs)
        {
            var lhsDistance = (position - lhs.transform.position).magnitude;
            var rhsDistance = (position - rhs.transform.position).magnitude;
            return (lhsDistance > rhsDistance) ? 1 : -1;
        });
        return enemies[0];
    }

    private void Start()
    {
        StartCoroutine(AttemptSpawn());
    }
}
