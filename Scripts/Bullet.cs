using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace com.nemodouble.massiveGunner.Scripts
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TrailRenderer trailRenderer;
        
        public float maxDamage = 0f;
        public float minDamage = 0f;
        public float damage = 0f;
        public float percentDamage = 0f;
        public float lifeTime = 0f;
        public int maxPierceCount = 1;
        private int _pierceCount = 0;
        
        private void Update()
        {
            lifeTime -= Time.deltaTime;
            damage = Mathf.Lerp(maxDamage, minDamage, 1 - lifeTime / this.lifeTime);
            if (lifeTime <= 0)
            {
                GameManger.Instance.bulletPool.ReturnObject(gameObject);
            }
        }

        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.TryGetComponent(typeof(IDamageable), out var obj))
            {
                ((IDamageable)obj).GetDamage(damage);
                ((IDamageable)obj).GetPercentageDamage(percentDamage);
            }
            if (++_pierceCount >= maxPierceCount)
            {
                GameManger.Instance.bulletPool.ReturnObject(gameObject);
            }
        }

        
        public void Init(float maxDamage, float minDamage, float percentDamage, float lifeTime, Gun.GunOwner owner = default, int pierceCount = 1)
        {
            damage = maxDamage;
            this.maxDamage = maxDamage;
            this.minDamage = minDamage;
            this.percentDamage = percentDamage;
            this.lifeTime = lifeTime;
            maxPierceCount = pierceCount;
            trailRenderer.Clear();
            if (owner == Gun.GunOwner.Enemy)
            {
                spriteRenderer.color = Color.red;
                trailRenderer.startColor = Color.red;
                trailRenderer.endColor = Color.red;
                gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
            }
            else
            {
                spriteRenderer.color = new Color(0xFF / 255f, 0xF6 / 255f, 0xA5 / 255f);
                trailRenderer.startColor = new Color(0xFF / 255f, 0xF6 / 255f, 0xA5 / 255f);
                trailRenderer.endColor = new Color(0xFF / 255f, 0xF6 / 255f, 0xA5 / 255f);
                gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
            }
        }
    }
}