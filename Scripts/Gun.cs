using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts
{
    public class Gun : MonoBehaviour
    {
        public enum GunOwner
        {
            Player,
            Enemy,
            None
        }
        [SerializeField] public GunOwner owner = GunOwner.Enemy;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private AudioClip shootSound;
        [SerializeField] private AudioClip reloadSound;

        [OnValueChanged("UpdateDps")] [SerializeField] private float rpm = 299f;
        [OnValueChanged("UpdateDps")] [SerializeField] private float bulletMaxDamage = 1f;
        [OnValueChanged("UpdateDps")] [SerializeField] private float bulletMinDamage = 0.5f;
        [OnValueChanged("UpdateDps")] [SerializeField] private int magazineSize = 10;
        [OnValueChanged("UpdateDps")] [SerializeField] private float reloadTime = 1f;
        [OnValueChanged("UpdateDps")] [SerializeField] private int pierceCount = 1;
        [ShowInInspector, ReadOnly] public float _gunDps;
        
        [Space(10)]
        [OnValueChanged("UpdateRange")] [SerializeField] private float bulletSpeed = 5f;
        [OnValueChanged("UpdateRange")] [SerializeField] private float bulletLifeTime = 2f;
        [ShowInInspector, ReadOnly] public float gunDistance;
        
        [Space(10)]
        [SerializeField]
        public float gunHealthMax = 500f;
        [SerializeField] private float gunHealthGeneration = 1f;
        private float gunHealthNow;
        
        // gun Sound
        
        private float fireCool = 0.5f;
        private float _nextFire;
        private int _currentMagazine;
        private bool _isReloading;
        private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");

        
        private void Start()
        {
            gunDistance = bulletSpeed * bulletLifeTime;
            _nextFire = fireCool;
            fireCool = 60f / rpm;
            gunHealthNow = gunHealthMax;
            spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateDps();
        }

        private void Update()
        {
            if(gunHealthNow < gunHealthMax)
                gunHealthNow += gunHealthGeneration * Time.deltaTime;
            if (owner != GunOwner.None)
            {
                if (_nextFire > 0)
                {
                    _nextFire -= Time.deltaTime;
                    if (_nextFire <= 0)
                    {
                        _nextFire = fireCool;
                        Shoot();
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (owner == GunOwner.None)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("Player") ||
                    col.gameObject.layer == LayerMask.NameToLayer("PlayersGun"))
                {
                    TakenByPlayer(col.gameObject);
                }
            }

            if (owner == GunOwner.Player)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
                {
                    if (GameManger.Instance.difficulty == ModeSelect.Difficulty.Hard)
                    {
                        gunHealthNow -= col.gameObject.GetComponent<Bullet>().damage;
                        spriteRenderer.color = new Color(gunHealthNow / gunHealthMax, 1, 1, 1);
                        GameManger.Instance.bulletPool.ReturnObject(col.gameObject);
                        if (gunHealthNow <= 0)
                        {
                            Destroy(gameObject);
                            GameManger.Instance.player.CleanupDestroyGuns();
                        }
                    }
                    else
                    {
                        GameManger.Instance.player.GetDamage(col.gameObject.GetComponent<Bullet>().damage);
                        GameManger.Instance.player.GetPercentageDamage(col.gameObject.GetComponent<Bullet>().percentDamage);
                        GameManger.Instance.bulletPool.ReturnObject(col.gameObject);
                    }
                }
            }
        }

        private void Shoot()
        {
            // If there is no bullet in the magazine, reload
            if (_currentMagazine <= 0)
            {
                if (!_isReloading) StartCoroutine(Reload());
                return;
            }
            
            // get bullet target pos
            Vector2 bulletTargetPos;
            if (owner == GunOwner.Player)
            {
                var mouseScreenPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
                bulletTargetPos = (Vector2)GameManger.Instance.mainCamera.ScreenToWorldPoint(mouseScreenPos);
            }
            else
            {
                var gunTransform = transform;
                var gunPos = gunTransform.position;
                var playerDistance = (GameManger.Instance.player.transform.position - gunPos).magnitude;
                if (playerDistance > gunDistance)
                    return;
                bulletTargetPos = (Vector2)gunPos + (Vector2)gunTransform.right * playerDistance;
            }
            var randomPos = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            bulletTargetPos += randomPos;
            
            // Bullet init
            var bulletObj = GameManger.Instance.bulletPool.GetObject();
            var myTransform = transform;
            bulletObj.transform.position = myTransform.position;
            bulletObj.transform.rotation = myTransform.rotation;
            if (bulletObj.TryGetComponent(typeof(Bullet), out var bullet))
            {
                ((Bullet)bullet).Init(bulletMaxDamage,  bulletMinDamage, 0, bulletLifeTime, owner, pierceCount);
            }
            
            if (bulletObj.TryGetComponent(typeof(Rigidbody2D), out var rb))
            {
                ((Rigidbody2D)rb).velocity = (bulletTargetPos - (Vector2)myTransform.position).normalized * bulletSpeed;
            }
            
            _currentMagazine--;
            // if(owner == GunOwner.Player)
            //     AudioSource.PlayClipAtPoint(shootSound, transform.position);
        }

        private IEnumerator Reload()
        {
            _isReloading = true;
            // if(owner == GunOwner.Player)
            //     AudioSource.PlayClipAtPoint(reloadSound, transform.position);
            yield return new WaitForSeconds(reloadTime);
            _currentMagazine = magazineSize;
            _isReloading = false;
        }

        private void TakenByPlayer(GameObject collidedObject)
        {
            owner = GunOwner.Player;
            bulletSpeed *= 5f;
            rpm *= 4f;
            fireCool = 60f / rpm;
            bulletLifeTime *= 0.5f;
            gameObject.layer = LayerMask.NameToLayer("PlayersGun");
            GameManger.Instance.player.PickUpGun(this, collidedObject);
        }

        private void UpdateDps()
        {
            fireCool = 60f / rpm;
            _gunDps = ((bulletMinDamage + bulletMaxDamage) * 0.5f * magazineSize) / (magazineSize * fireCool + reloadTime);
        }
        
        private void UpdateRange()
        {
            gunDistance = bulletSpeed * bulletLifeTime;
        }
    }
}
