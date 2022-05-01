using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stage
{
    public class Floor : MonoBehaviour
    {
        private float spawnRate = 0.01f;

        private IEnumerator AttemptSpawn()
        {
            for (; ; )
            {
                Spawn();
                yield return new WaitForSeconds(1);
            }
        }

        private void Spawn()
        {
            // if (Random.value < spawnRate)
            // {
            //     EnemiesManager.instance.Spawn(transform.position);
            // }
        }

        // Update is called once per frame
        void Start()
        {
            StartCoroutine("AttemptSpawn");
        }
    }
}
