using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts.Boss
{
    public class Grenade : MonoBehaviour
    {
        private float delay = 2.5f;
        private float damage = 0.25f;
        private int fragnentCount = 3;
        private float fragnentDelay = 0.5f;
        private float fragnentDamage = 0.1f;
        private float knockbackSpeed = 40f;
        private float knockbackDuration = 0.1f;
        public CircleCollider2D circleCollider2D;
        public GameObject fragPrefab;
        
        private void OnEnable()
        {
            StartCoroutine(GrenadeLifeCycle());
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.TryGetComponent(typeof(IDamageable), out var obj))
            {
                ((IDamageable)obj).GetPercentageDamage(damage);
            }
            if (col.gameObject.TryGetComponent(typeof(Player), out var obj2))
            {
                var player = (Player)obj2;
                var direction = player.transform.position - transform.position;
                player.PushBack(direction.normalized, knockbackSpeed, knockbackDuration);
            }
        }
        
        private System.Collections.IEnumerator GrenadeLifeCycle()
        {
            yield return new WaitForSeconds(delay);
            circleCollider2D.enabled = true;
            for (var i = 0; i < fragnentCount; i++)
            {
                var angle = 360 / fragnentCount * i;
                var fragPosition = transform.position +
                                   new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * 4f;
                Instantiate(fragPrefab, fragPosition, Quaternion.identity)
                    .GetComponent<Fragment>().Init(fragnentDelay * i, fragnentDamage);
            }
            yield return null;
            Destroy(gameObject);
        }
        
        public void Init(float delay, float damage, int fragnentCount, float fragnentDelay, float fragnentDamage, float knockbackSpeed, float knockbackDuration)
        {
            this.delay = delay;
            this.damage = damage;
            this.fragnentCount = fragnentCount;
            this.fragnentDelay = fragnentDelay;
            this.fragnentDamage = fragnentDamage;
            this.knockbackSpeed = knockbackSpeed;
            this.knockbackDuration = knockbackDuration;
        }
    }
}