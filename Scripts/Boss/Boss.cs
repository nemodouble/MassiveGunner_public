using System;
using System.Collections;
using Cinemachine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace com.nemodouble.massiveGunner.Scripts.Boss
{
    public class Boss : MonoBehaviour, IDamageable
    {
        [FoldoutGroup("General")] public float maxHealth = 100000;
        [FoldoutGroup("General")] public float damage = 0.05f;
        [FoldoutGroup("General")] public float Phase2HealthRate = 0.57f;
        [FoldoutGroup("General")] public GameObject bulletPrefab;
        [FoldoutGroup("General")] public Slider bossHpSlider;
        [FoldoutGroup("General")] public TextMeshProUGUI bossHpText;
        
        [FoldoutGroup("Camera")] public CinemachineVirtualCamera bossCamera;
        [FoldoutGroup("Camera")] public CinemachineVirtualCamera middleCam;
        private float currentHealth;

        [FoldoutGroup("Phase0")] public float ph0WaitingTime = 1f;
        [FoldoutGroup("Phase0")] public float ph0CloseUpSize = 2f;
        private float cameraOriginalSize;

        [FoldoutGroup("Phase1Pattern0")] public float ph1p0movingSpeedRate = 1f;
        [FoldoutGroup("Phase1Pattern0")] public float ph1p0moveDurationMin = 3f;
        [FoldoutGroup("Phase1Pattern0")] public float ph1p0moveDurationMax = 5f;
        
        private float ph1p1UsedTime = 0;
        [FoldoutGroup("Phase1Pattern1")] public float ph1p1Cool = 20f;
        [FoldoutGroup("Phase1Pattern1")] public float ph1p1Range = 15f;
        [FoldoutGroup("Phase1Pattern1")] public float ph1p1BeforeDelay = 0.3f;
        [FoldoutGroup("Phase1Pattern1")] public float ph1p1PassDistance = 10f;
        [FoldoutGroup("Phase1Pattern1")] public float ph1p1movingSpeed = 11f;
        [FoldoutGroup("Phase1Pattern1")] public float ph1p1Damage = 0.10f;
        [FoldoutGroup("Phase1Pattern1")] public GameObject ph1p1WayPreview;
        
        private float ph1p2UsedTime = 0;
        [FoldoutGroup("Phase1Pattern2")] public float ph1p2Cool = 40f;
        [FoldoutGroup("Phase1Pattern2")] public float ph1p2BombCool = 0.25f;
        [FoldoutGroup("Phase1Pattern2")] public GameObject ph1p2TriggerHorizonRange;
        [FoldoutGroup("Phase1Pattern2")] public GameObject ph1p2TriggerVerticalRange;

        [FoldoutGroup("Phase1Pattern3")] public float ph1p3Cool = 5f;
        [FoldoutGroup("Phase1Pattern3")] public Transform leftGunPos;
        [FoldoutGroup("Phase1Pattern3")] public Transform rightGunPos;
        [FoldoutGroup("Phase1Pattern3")] public float ph1p3BulletSpeed = 1.5f;
        [FoldoutGroup("Phase1Pattern3")] public float ph1p3BulletLifeTime = 3f;
        [FoldoutGroup("Phase1Pattern3")] public float ph1p3BulletDamage = 0.15f;
        [FoldoutGroup("Phase1Pattern3")] public float ph1p3BulletCool = 0.25f;
        
        [FoldoutGroup("Phase2Pattern0")] public float ph2p0MoveSpeed = 1f;
        [FoldoutGroup("Phase2Pattern0")] public float ph2p0MoveDuration = 2f;
        [FoldoutGroup("Phase2Pattern0")] public float ph2p0BulletCount = 5f;
        [FoldoutGroup("Phase2Pattern0")] public float ph2p0BulletDamage = 0.05f;
        [FoldoutGroup("Phase2Pattern0")] public float ph2p0BulletLifeTime = 3f;
        [FoldoutGroup("Phase2Pattern0")] public float ph2p0BulletSpeed = 1.5f;
        
        [FoldoutGroup("Phase2Pattern1")] public float ph2p1Cool = 5f;
        [FoldoutGroup("Phase2Pattern1")] public float ph2p1TrapDuration = 10f;
        [FoldoutGroup("Phase2Pattern1")] public float ph2p1TrapDelay = 1f;
        [FoldoutGroup("Phase2Pattern1")] public float ph2p1TrapDamage = 0.05f;
        [FoldoutGroup("Phase2Pattern1")] public float ph2p1TrapSlow = 0.66f;
        [FoldoutGroup("Phase2Pattern1")] public float ph2p1TrapSlowDuration = 2f;
        [FoldoutGroup("Phase2Pattern1")] public GameObject ph2p1TrapPrefab;
        
        [FoldoutGroup("Phase2Pattern2")] public float ph2p2Delay = 2f;
        [FoldoutGroup("Phase2Pattern2")] public float ph2p2Duration = 5f;
        [FoldoutGroup("Phase2Pattern2")] public float ph2p2Damage = 0.05f;
        [FoldoutGroup("Phase2Pattern2")] public float ph2p2BulletCount = 77f;
        [FoldoutGroup("Phase2Pattern2")] public float ph2p2BulletSpeed = 15f;
        [FoldoutGroup("Phase2Pattern2")] public float ph2p2BulletLifeTime = 3f;
        [FoldoutGroup("Phase2Pattern2")] public float ph2p2AimFollowSpeedRatePlayer = 10f;
        [FoldoutGroup("Phase2Pattern2")] public GameObject ph2p2Range;
        
        [FoldoutGroup("Phase2Pattern3")] public float ph2p3InvisibleTime = 2f;
        [FoldoutGroup("Phase2Pattern3")] public float ph2p3Delay = 2f;
        [FoldoutGroup("Phase2Pattern3")] public float ph2p3Duration = 3f;
        [FoldoutGroup("Phase2Pattern3")] public float ph2p3BulletCount = 5f;
        [FoldoutGroup("Phase2Pattern3")] public float ph2p3BulletDamage = 0.3f;
        [FoldoutGroup("Phase2Pattern3")] public float ph2p3LastShootAditionalDelay = 0.5f;
        [FoldoutGroup("Phase2Pattern3")] public GameObject ph2p3BulletRangePrefab;
        
        [FoldoutGroup("Phase2Pattern4")] public float ph2p4Cool = 30f;
        [FoldoutGroup("Phase2Pattern4")] public int ph2p4FireCount = 2;
        [FoldoutGroup("Phase2Pattern4")] public float ph2p4Delay = 1f;
        [FoldoutGroup("Phase2Pattern4")] public float ph2p4Damage = 0.0001f;
        [FoldoutGroup("Phase2Pattern4")] public float ph2p4SpeedUpRate = 1.5f;
        [FoldoutGroup("Phase2Pattern4")] public float ph2p4SpeedUpDuration = 15f;
        [FoldoutGroup("Phase2Pattern4")] public float ph2p4TrapDuration = 30f;
        [FoldoutGroup("Phase2Pattern4")] public GameObject ph2p4FirePrefab;
        
        [FoldoutGroup("Phase2Pattern5")] public float ph2p5GrenateCount = 5f;
        [FoldoutGroup("Phase2Pattern5")] public int ph2p5FragCount = 3;
        [FoldoutGroup("Phase2Pattern5")] public float ph2p5Delay = 2.5f;
        [FoldoutGroup("Phase2Pattern5")] public float ph2p5grenateDamage = 0.3f;
        [FoldoutGroup("Phase2Pattern5")] public float ph2p5fragDamage = 0.1f;
        [FoldoutGroup("Phase2Pattern5")] public float ph2p5fragDelay = 0.5f;
        [FoldoutGroup("Phase2Pattern5")] public float ph2p5KnockBackSpeed = 40f;
        [FoldoutGroup("Phase2Pattern5")] public float ph2p5KnockBackDuration = 0.1f;
        [FoldoutGroup("Phase2Pattern5")] public GameObject ph2p5GrenatePrefab;
        
        [FoldoutGroup("Phase2Pattern6")] public float ph2p6Delay = 3f;
        [FoldoutGroup("Phase2Pattern6")] public GameObject sub1;
        [FoldoutGroup("Phase2Pattern6")] public GameObject sub2;

        private float playerSpeed;
        
        private Coroutine bossCoroutine;
        private Coroutine bossSubCoroutine1;
        private Coroutine bossSubCoroutine2;

        private GameObject _damageTextGameObject;

        // Debug
        public string bossPattern = "Phase1P0";
        
        
        private void Awake()
        {
            cameraOriginalSize = bossCamera.m_Lens.OrthographicSize;
            currentHealth = maxHealth;
        }

        private void Start()
        {
            playerSpeed = GameManger.Instance.player.GetNowSpeed();
            bossCoroutine = StartCoroutine(StartBoss());
            bossHpSlider.maxValue = maxHealth;
            bossHpSlider.value = currentHealth;
            bossHpSlider.gameObject.SetActive(true);
            bossHpText.text = GameManger.Instance.ToKmbNumber(currentHealth) + "/" + GameManger.Instance.ToKmbNumber(maxHealth);
            bossHpText.gameObject.SetActive(true);
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<IDamageable>().GetPercentageDamage(damage);
            }
        }
        
        private IEnumerator StartBoss()
        {
            transform.position = new Vector2(25f, -25f);
            bossCamera.Priority = 11;
            var timer = 0f;
            while (timer < ph0WaitingTime)
            {
                timer += Time.deltaTime;
                bossCamera.m_Lens.OrthographicSize = Mathf.Lerp(cameraOriginalSize, ph0CloseUpSize, timer / ph0WaitingTime);
                yield return null;
            }
            bossCamera.m_Lens.OrthographicSize = cameraOriginalSize;
            bossCamera.Priority = 9;
            StartPhase1();
        }

        private void StartPhase1()
        {
            StopCoroutine(bossCoroutine);
            bossCoroutine = StartCoroutine(Phase1P0());
            bossSubCoroutine1 = StartCoroutine(Phase1P3());
        }

        private bool StartPh1Pattern()
        {
            var toPlayerDistance = Vector2.Distance(transform.position, GameManger.Instance.player.transform.position);
            if(Time.time - ph1p1UsedTime > ph1p1Cool && toPlayerDistance < ph1p1Range)
            {
                StopCoroutine(bossCoroutine);
                bossCoroutine = StartCoroutine(Phase1P1());
                return true;
            }
            if(Time.time - ph1p2UsedTime > ph1p2Cool)
            {
                bossCoroutine = StartCoroutine(Phase1P2());
                return true;
            }

            return false;
        }

        private IEnumerator Phase1P0()
        {
            //Debug
            bossPattern = "Phase1P0";

            while (currentHealth > maxHealth * Phase2HealthRate)
            {
                var movingDuration = Random.Range(ph1p0moveDurationMin, ph1p0moveDurationMax);
                var moveWay = Random.Range(0, 2);
                var timer = 0f;
                while (timer < movingDuration)
                {
                    var playerPos = FindObjectOfType<Player>().transform.position;
                    var toPlayer = playerPos - transform.position;
                    var moveDirection = new Vector2(-toPlayer.y, toPlayer.x) * (moveWay == 0 ? -1 : 1);
                    transform.position += (Vector3)moveDirection.normalized * ph1p0movingSpeedRate * playerSpeed * Time.deltaTime;
                    timer += Time.deltaTime;
                    if(StartPh1Pattern()) yield break;
                    yield return null;
                }
            }
            bossCoroutine = StartCoroutine(StartPhase2());
        }
        
        //Debug
        [Button]
        private void DebugPhase1P1()
        {
            StopCoroutine(bossCoroutine);
            bossCoroutine = StartCoroutine(Phase1P1());
        }
        
        private IEnumerator Phase1P1()
        {
            //Debug
            bossPattern = "Phase1P1";
            
            StopCoroutine(bossSubCoroutine1);
            bossSubCoroutine1 = null;

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
            bossSubCoroutine1 = StartCoroutine(Phase1P3());
            bossCoroutine = StartCoroutine(Phase1P0());
        }
        
        //Debug
        [Button]
        private void DebugPhase1P2()
        {
            StopCoroutine(bossCoroutine);
            bossCoroutine = StartCoroutine(Phase1P2());
        }
        
        private IEnumerator Phase1P2()
        {
            //Debug
            bossPattern = "Phase1P2";
            
            StopCoroutine(bossSubCoroutine1);
            bossSubCoroutine1 = null;

            var startPointSelect = Random.Range(0, 4);
            var moveDirection = Vector2.zero;
            switch (startPointSelect)
            {
                case 0:
                    moveDirection = Vector2.up;
                    ph1p2TriggerHorizonRange.transform.position = new Vector2(25, -50);
                    break;
                case 1:
                    moveDirection = Vector2.down;
                    ph1p2TriggerHorizonRange.transform.position = new Vector2(25, 0);
                    break;
                case 2:
                    moveDirection = Vector2.right;
                    ph1p2TriggerVerticalRange.transform.position = new Vector2(0, -25);
                    break;
                case 3:
                    moveDirection = Vector2.left;
                    ph1p2TriggerVerticalRange.transform.position = new Vector2(50, -25);
                    break;
            }

            var range = startPointSelect < 2 ? ph1p2TriggerHorizonRange : ph1p2TriggerVerticalRange;
            var boxColliderX = range.GetComponent<BoxCollider2D>().size.x;
            for (var i = 0; i < 15; i++)
            {
                range.SetActive(true);
                yield return null;
                yield return null;
                yield return null;
                range.SetActive(false);
                range.transform.position += (Vector3)moveDirection * boxColliderX;
                yield return new WaitForSeconds(ph1p2BombCool);
            }
            
            yield return null;
            ph1p2UsedTime = Time.time;
            bossSubCoroutine1 = StartCoroutine(Phase1P3());
            bossCoroutine = StartCoroutine(Phase1P0());
        }
        
        private IEnumerator Phase1P3()
        {
            // Debug
            bossPattern = "Phase1P3";
            
            while (true)
            {
                yield return new WaitForSeconds(ph1p3Cool);
                for (var i = 0; i < 2; i++)
                {
                    for (var j = 0; j < 6; j++)
                    {
                        var bullet = Instantiate(bulletPrefab);
                        bullet.transform.position = j < 3 ? leftGunPos.position : rightGunPos.position;
                        var bulletDirection = GameManger.Instance.player.transform.position - bullet.transform.position;
                        bullet.transform.rotation = Quaternion.Euler(0, 0,
                            Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg);
                        bullet.GetComponent<Bullet>()
                            .Init(0, 0, ph1p3BulletDamage, ph1p3BulletLifeTime, Gun.GunOwner.Enemy);
                        bullet.GetComponent<Rigidbody2D>().velocity = bulletDirection.normalized * ph1p3BulletSpeed;
                        yield return new WaitForSeconds(ph1p3BulletCool);
                    }
                }
            }
        }
        
        //Debug
        [Button]
        private void DebugPhase2()
        {
            StopCoroutine(bossCoroutine);
            bossCoroutine = StartCoroutine(StartPhase2());
        }
        
        private IEnumerator StartPhase2()
        {
            StopCoroutine(bossCoroutine);
            StopCoroutine(bossSubCoroutine1);
            bossSubCoroutine1 = null;

            // animation
            yield return null; 
            
            bossCoroutine = StartCoroutine(Phase2P3());
            bossSubCoroutine1 = StartCoroutine(Phase2P1());
            bossSubCoroutine2 = StartCoroutine(Phase2P4());
        }
        
        //Debug
        [Button]
        private void DebugPhase2P0()
        {
            StopCoroutine(bossCoroutine);
            bossCoroutine = StartCoroutine(Phase2P0());
        }
        
        private IEnumerator Phase2P0()
        {
            //Debug
            bossPattern = "Phase2P0";
            
            var randomAngle = Random.Range(-180f, 180f);
            var randomMoveDir = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
            var timer = 0f;
            var shootTimer = 0f;
            while (timer < ph2p0MoveDuration)
            {
                transform.position += (Vector3)randomMoveDir * ph2p0MoveSpeed * Time.deltaTime;
                timer += Time.deltaTime;
                
                shootTimer += Time.deltaTime;
                if (shootTimer > ph2p0MoveDuration / ph2p0BulletCount)
                {
                    var bullet = Instantiate(bulletPrefab);
                    bullet.transform.position = transform.position;
                    var bulletDirection = GameManger.Instance.player.transform.position - bullet.transform.position;
                    bullet.transform.rotation = Quaternion.Euler(0, 0,
                        Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg);
                    var bulletComponent = bullet.GetComponent<Bullet>();
                    bulletComponent.Init(0, 0, ph2p0BulletDamage, ph2p0BulletLifeTime, Gun.GunOwner.Enemy);
                    bullet.GetComponent<Rigidbody2D>().velocity = bulletDirection.normalized * ph2p0BulletSpeed;
                    shootTimer -= ph2p0MoveDuration / ph2p0BulletCount;
                }
                yield return null;
            }
            
            var nextPattern = Random.Range(0, 3);
            bossCoroutine = nextPattern switch
            {
                0 => StartCoroutine(Phase2P2()),
                1 => StartCoroutine(Phase2P3()),
                2 => StartCoroutine(Phase2P5()),
                _ => bossCoroutine
            };
        }
        
        private IEnumerator Phase2P1()
        {
            while (true)
            {
                yield return new WaitForSeconds(ph2p1Cool);
                var randomPos = new Vector2(Random.Range(0f, 50f), Random.Range(-50f, 0f));
                var trap = Instantiate(ph2p1TrapPrefab, GameManger.Instance.player.transform.position, Quaternion.identity);
                trap.GetComponent<Trap>().Init(ph2p1TrapDelay, ph2p1TrapDamage, ph2p1TrapSlow, ph2p1TrapSlowDuration, ph2p1TrapDuration);
                trap.transform.position = randomPos;
            }
        }
        
        //Debug
        [Button]
        private void DebugPhase2P2()
        {
            StopCoroutine(bossCoroutine);
            bossCoroutine = StartCoroutine(Phase2P2());
        }
        
        private IEnumerator Phase2P2()
        {
            //Debug
            bossPattern = "Phase2P2";
            
            ph2p2Range.SetActive(true);
            
            var timer = 0f;
            var shootTimer = 0f;
            var player = GameManger.Instance.player;
            var targetPos = player.transform.position;
            while (timer < ph2p2Delay + ph2p2Duration)
            {
                timer += Time.deltaTime;
                targetPos += (player.transform.position - targetPos).normalized * playerSpeed *
                             ph2p2AimFollowSpeedRatePlayer * Time.deltaTime;
                var toTarget = targetPos - transform.position;
                ph2p2Range.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg + 90);
                if (timer >= ph2p2Delay)
                {
                    shootTimer += Time.deltaTime;
                    if (shootTimer > ph2p2Duration / ph2p2BulletCount)
                    {
                        var bullet = Instantiate(bulletPrefab);
                        bullet.transform.position = transform.position;
                        bullet.transform.rotation = ph2p2Range.transform.rotation;
                        var bulletDirection = targetPos - bullet.transform.position;
                        bullet.GetComponent<Bullet>().Init(0, 0, ph2p2Damage, ph2p2BulletLifeTime, Gun.GunOwner.Enemy);
                        bullet.GetComponent<Rigidbody2D>().velocity = bulletDirection.normalized * ph2p2BulletSpeed;
                        shootTimer -= ph2p2Duration / ph2p2BulletCount;
                    }
                }
                yield return null;
            }
            ph2p2Range.SetActive(false);
            bossCoroutine = StartCoroutine(Phase2P0());
        } 
        
        //Debug
        [Button]
        private void DebugPhase2P3()
        {
            StopCoroutine(bossCoroutine);
            bossCoroutine = StartCoroutine(Phase2P3());
        }
        
        private IEnumerator Phase2P3()
        {
            //Debug
            bossPattern = "Phase2P3";

            middleCam.Priority = 11;
            GameManger.Instance.mainCamera.transform.position = new Vector3(25f, -25f, -10f);
            var targetPos = new Vector2(25f, -25f);
            var timer = 0f;
            while (timer < ph2p3InvisibleTime)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPos, playerSpeed * Time.unscaledDeltaTime);
                timer += Time.unscaledDeltaTime;
                
                yield return null;
            }
            Time.timeScale = 1f;
            middleCam.Priority = 9;

            var bulletDelay = ph2p3Duration / ph2p3BulletCount;
            GameObject go;
            for (var i = 0; i < ph2p3BulletCount - 1; i++)
            {
                go = Instantiate(ph2p3BulletRangePrefab, GameManger.Instance.player.transform.position, Quaternion.identity);
                go.GetComponent<SnipeRange>().Init(ph2p3Delay, ph2p3BulletDamage);
                yield return new WaitForSeconds(bulletDelay);
            }

            yield return new WaitForSeconds(ph2p3LastShootAditionalDelay);
            var predictedPlayerPos = (Vector2)GameManger.Instance.player.transform.position + GameManger.Instance.player.GetNowDirection() * ph2p3Delay;
            go = Instantiate(ph2p3BulletRangePrefab, predictedPlayerPos, Quaternion.identity);
            go.GetComponent<SnipeRange>().Init(ph2p3Delay, ph2p3BulletDamage);
            
            bossCoroutine = StartCoroutine(Phase2P0());
        }
        
        private IEnumerator Phase2P4()
        {
            //Debug
            bossPattern = "Phase2P4";
            while (true)
            {
                for(var i = 0; i < ph2p4FireCount; i++)
                {
                    var randomPos = new Vector2(Random.Range(0f, 50f), Random.Range(-50f, 0f));
                    var go = Instantiate(ph2p4FirePrefab, randomPos, Quaternion.identity);
                    go.GetComponent<FireRange>().Init(ph2p4Delay, ph2p4Damage, ph2p4SpeedUpRate,
                        ph2p4SpeedUpDuration, ph2p4TrapDuration);
                }
                yield return new WaitForSeconds(ph2p4Cool);
            }
        }
        
        //Debug
        [Button]
        private void DebugPhase2P5()
        {
            StopCoroutine(bossCoroutine);
            bossCoroutine = StartCoroutine(Phase2P5());
        }
        
        private IEnumerator Phase2P5()
        {
            for (var i = 0; i < ph2p5GrenateCount; i++)
            {
                var randomPos = new Vector2(Random.Range(0f, 50f), Random.Range(-50f, 0f));
                var go = Instantiate(ph2p5GrenatePrefab, randomPos, Quaternion.identity);
                go.GetComponent<Grenade>().Init(ph2p5Delay, ph2p5grenateDamage, ph2p5FragCount, ph2p5fragDelay,
                    ph2p5fragDamage, ph2p5KnockBackSpeed, ph2p5KnockBackDuration);
                yield return null;
            }
            bossCoroutine = StartCoroutine(Phase2P0());
        }
        
        //Debug
        [Button]
        private void DebugPhase2P6()
        {
            StopCoroutine(bossCoroutine);
            bossCoroutine = StartCoroutine(Phase2P6());
        }
        
        private IEnumerator Phase2P6()
        {
            //Debug
            bossPattern = "Phase2P6";
            
            yield return new WaitForSeconds(ph2p6Delay);
            sub1.SetActive(true);
            sub2.SetActive(true);
            yield return null;
        }

        public void GetDamage(float damage)
        {
            currentHealth -= damage;
            
            PrintDamageText(damage);
            
            if (currentHealth <= 0)
            {
                GameManger.Instance.WinGame();
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