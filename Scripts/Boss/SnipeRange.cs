using System.Collections;
using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts.Boss
{
    public class SnipeRange : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boxCollider2D;
        private float damage = 0.3f;
        private float delay = 2f;
        
        private void OnEnable()
        {
            StopCoroutine(TrapLifeCycle());
            StartCoroutine(TrapLifeCycle());
        }
        
        private IEnumerator TrapLifeCycle()
        {
            boxCollider2D.enabled = false;
            yield return new WaitForSeconds(delay);
            boxCollider2D.enabled = true;
            yield return null;
            boxCollider2D.enabled = false;
            
            Destroy(gameObject);
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.TryGetComponent(typeof(IDamageable), out var obj))
            {
                ((IDamageable)obj).GetPercentageDamage(damage);
            }
        }
        
        public void Init(float delay, float damage)
        {
            this.delay = delay;
            this.damage = damage;
        }
    }
}