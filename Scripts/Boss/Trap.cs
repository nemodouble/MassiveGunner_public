using System;
using System.Collections;
using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts.Boss
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boxCollider2D;

        private float activeDelay = 1f;
        private float trapDuration = 10f;
        protected float percentDamage = 0.05f;
        protected float speedChangeRate = 0.33f;
        protected float slowDuration = 2f;

        private void OnEnable()
        {
            StartCoroutine(TrapLifeCycle());
        }

        private IEnumerator TrapLifeCycle()
        {
            yield return new WaitForSeconds(activeDelay);
            boxCollider2D.enabled = true;
            yield return new WaitForSeconds(trapDuration);
            boxCollider2D.enabled = false;
            
            Destroy(gameObject);
        }

        protected virtual void OnTriggerEnter2D(Collider2D col)
        {
            if(col.gameObject.TryGetComponent(typeof(Player), out var ob))
            {
                ((Player)ob).StartSpiked(slowDuration, speedChangeRate);
            }
            
            if (col.gameObject.TryGetComponent(typeof(IDamageable), out var obj))
            {
                ((IDamageable)obj).GetPercentageDamage(percentDamage);
            }
        }

        public void Init(float activeDelay, float percentDamage, float slowRate, float slowDuration, float trapDuration)
        {
            this.activeDelay = activeDelay;
            this.percentDamage = percentDamage;
            this.speedChangeRate = slowRate;
            this.slowDuration = slowDuration;
            this.trapDuration = trapDuration;
        }
    }
}