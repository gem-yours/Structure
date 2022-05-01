using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Stage
{


    public class Wall : MonoBehaviour
    {
        public float hp { private set; get; } = 1;

        public void OnHit(float damage)
        {
            if (damage <= 0) return;
            hp -= damage;
            StartCoroutine(OnHitAnimation());
            checkHP();
        }

        private void checkHP()
        {
            // TODO: 壊れかけの状態を導入する
            if (hp > 0) return;
            Destroy(gameObject);
        }

        IEnumerator OnHitAnimation()
        {
            yield return null;
        }
    }
}