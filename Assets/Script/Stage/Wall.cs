using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Stage
{
    public class Wall : MonoBehaviour
    {
        public float hp { private set; get; } = 2;

        private SpriteRenderer spriteRenderer;

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
            Instantiate(Resources.Load("Map/Tile"), transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        IEnumerator OnHitAnimation()
        {
            var duration = 0.1f;
            yield return AnimationUtil.EaseInOut(duration, (current) =>
            {
                spriteRenderer.color = new Color(1, current, current, 1);
            });
            yield return AnimationUtil.EaseInOut(duration, (current) =>
            {
                spriteRenderer.color = new Color(1, 1 - current, 1 - current, 1);
            });
        }

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}