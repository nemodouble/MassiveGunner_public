using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts.Boss
{
    public class Sub : MonoBehaviour
    {
        private float ph1p1UsedTime = 0;
        [FoldoutGroup("Phase1Pattern1")] public float ph1p1Cool = 20f;
        [FoldoutGroup("Phase1Pattern1")] public float ph1p1Range = 15f;
        [FoldoutGroup("Phase1Pattern1")] public float ph1p1BeforeDelay = 0.3f;
        [FoldoutGroup("Phase1Pattern1")] public float ph1p1PassDistance = 10f;
        [FoldoutGroup("Phase1Pattern1")] public float ph1p1movingSpeed = 11f;
        [FoldoutGroup("Phase1Pattern1")] public float ph1p1Damage = 0.10f;
        [FoldoutGroup("Phase1Pattern1")] public GameObject ph1p1WayPreview;
        
        private Coroutine bossCoroutine;
        private float damage = 0.05f;

        protected virtual IEnumerator Pattern1()
        {
            yield return null;
        }
        
        private IEnumerator Pattern2()
        {
            var playerPos = GameManger.Instance.player.transform.position;
            var direction = playerPos - transform.position;
            var targetPos = playerPos + direction.normalized * ph1p1PassDistance;
            ph1p1WayPreview.SetActive(true);
            ph1p1WayPreview.transform.rotation =
                Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90);
            yield return new WaitForSeconds(ph1p1BeforeDelay);
            ph1p1WayPreview.SetActive(false);
            var originBodyDamage = damage;
            damage = ph1p1Damage;
            while (Vector2.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPos, ph1p1movingSpeed * Time.deltaTime);
                yield return null;
            }
            damage = originBodyDamage;
            
            ph1p1UsedTime = Time.time;
            bossCoroutine = StartCoroutine(Pattern1());
        }
    }
}