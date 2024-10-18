using System.Collections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [Header("Components")]
        [SerializeField] private Rigidbody2D _rb;
        [InlineEditor]
        [SerializeField] private Gun gun;
        [SerializeField] private Transform gunHolder;
        
        [Header("Stats")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float aimFallowSpeed;
        [SerializeField] public float maxHealth;
        [SerializeField] private float gunDropChance = 0.05f;
        
        [ReadOnly] public float _currentHealth;
        private Transform _player;
        private Vector2 _moveDirection;
        private Vector2 _aimDirection;
        
        private GameObject _damageTextGameObject;

        private void Start()
        {
            _currentHealth = maxHealth;
            _player = GameManger.Instance.player.transform;
        }
        
        private void Update()
        {
            if(gun.owner == Gun.GunOwner.Enemy)
            {
                MoveToPlayer();
                AimGun();
            }
        }

        public void GetDamage(float damage)
        {
            _currentHealth -= damage;
            
            PrintDamageText(damage);
            
            StatisticManager.Instance.statisticData.totalDamage += damage;
            
            if (_currentHealth <= 0)
            {
                StatisticManager.Instance.statisticData.totalKills += 1;
                StartCoroutine(Dead());
            }
        }

        public void GetPercentageDamage(float percentage)
        {
            GetDamage(maxHealth * percentage);
        }

        public void DamageTextDead()
        {
            _damageTextGameObject = null;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        private void AimGun()
        {
            _aimDirection = _player.position - gun.transform.position;
            var angle = Mathf.Atan2(_aimDirection.y, _aimDirection.x) * Mathf.Rad2Deg;
            var angleDiff = Mathf.DeltaAngle(gun.transform.eulerAngles.z, angle);
            if (angleDiff > 0)
            {
                gunHolder.Rotate(Vector3.forward, aimFallowSpeed * Time.deltaTime);
            }
            else
            {
                gunHolder.Rotate(Vector3.forward, -aimFallowSpeed * Time.deltaTime);
            }
        }

        private void MoveToPlayer()
        {
            _moveDirection = transform.position - _player.position;
            
            if (gun.gunDistance * 0.7 < _moveDirection.magnitude && _moveDirection.magnitude < gun.gunDistance * 1.0f)
            {
                _rb.velocity = Vector2.zero;
                return;
            }
            if (_moveDirection.magnitude > gun.gunDistance * 0.7f) 
                _moveDirection = -_moveDirection;
            
            _rb.velocity = _moveDirection.normalized * moveSpeed;
        }
        
        private IEnumerator Dead()
        {
            if(gun.owner == Gun.GunOwner.Enemy)
            {
                if (GunDropRateManager.Instance.TryDrop(gunDropChance))
                {
                    gun.transform.parent = null;
                    gun.owner = Gun.GunOwner.None;
                }
            }
            // Why does Destroy(gameObject) sometimes disable the gun's components if there is no frame gap?
            yield return null;
            Destroy(gameObject);
        }
        
        private void PrintDamageText(float damage)
        {
            if (_damageTextGameObject == null)
            {
                _damageTextGameObject = GameManger.Instance.damageTextPool.GetObject();
            }
            _damageTextGameObject.GetComponent<DamageText>().AddDamage(damage, this);
        }
    }
}