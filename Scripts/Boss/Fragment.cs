using System;
using System.Collections;
using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts.Boss
{
    public class Fragment : MonoBehaviour
    {
        public CircleCollider2D circleCollider2D;
        private float _fragmentDelay;
        private float _fragmentDamage;

        private void OnEnable()
        {
            StopCoroutine(FragmentLifeCycle());
            StartCoroutine(FragmentLifeCycle());
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.TryGetComponent(typeof(IDamageable), out var obj))
            {
                ((IDamageable)obj).GetPercentageDamage(_fragmentDamage);
            }
        }

        private IEnumerator FragmentLifeCycle()
        {
            yield return new WaitForSeconds(_fragmentDelay);
            circleCollider2D.enabled = true;
            yield return null;
            Destroy(gameObject);
        }

        public void Init(float fragmentDelay, float fragmentDamage)
        {
            _fragmentDelay = fragmentDelay;
            _fragmentDamage = fragmentDamage;
        }
        
    }
}