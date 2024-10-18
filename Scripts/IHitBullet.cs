using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts
{
    public interface IDamageable
    {
        public void GetDamage(float damage);
        public void GetPercentageDamage(float percentage);
        
        public void DamageTextDead();
        
        public GameObject GetGameObject();
    }
}