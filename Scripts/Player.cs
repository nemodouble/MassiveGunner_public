using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace com.nemodouble.massiveGunner.Scripts
{
    public class Player : MonoBehaviour, IDamageable, IBurnable
    {
        [Header("Player Components")]
        [SerializeField] private CinemachineBrain cinemachineBrain;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Transform gunHolder;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI dpsText;
        private StringBuilder _sb = new StringBuilder();
        private Vector2 _moveDirection;
        private bool canMove = true;

        [Header("Player Stats")]
        [SerializeField] private float originMoveSpeed = 5f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gunMoveSpeedBonus = 0.1f;
        [SerializeField] private float maxHealth = 100f;
        private float _currentHealth;
        private List<Gun> _guns = new();
        private Gun _farthestGun;
        private float _farthestGunDistance;
        private float _targetCameraSize = 10.45f;
        private float _nowCameraSize = 10.45f;
        
        public bool isBurning = false;
        private float buringDamage;
        private float burningDuration;
        private float burnSpeedRate;
        public bool isSpiked = false;
        private float spikedDuration;
        private float spikeSpeedRate;

        private float _totalDps;

        private void Start()
        {
            _currentHealth = maxHealth;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = _currentHealth;
            _sb.Clear();
            healthText.text = _sb.Append(_currentHealth.ToString("F0")).Append("/").Append(maxHealth).ToString();
        }

        private void Update()
        {
            var slowRate = 1 * (isBurning ? burnSpeedRate : 1) * (isSpiked ? spikeSpeedRate : 1);
            if(canMove) rb.velocity = _moveDirection * moveSpeed * slowRate;
            // set camera size
            if(Math.Abs(_nowCameraSize - _targetCameraSize) > 0.01f)
            {
                var clearShot = cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject
                    .GetComponent<CinemachineClearShot>();
                var virtualCamera = clearShot.LiveChild.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
                _nowCameraSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, _targetCameraSize, Time.deltaTime);
                virtualCamera.m_Lens.OrthographicSize = _nowCameraSize;
            }
            
            //burning
            if (isBurning)
            {
                GetDamage(buringDamage * Time.deltaTime);
                burningDuration -= Time.deltaTime;
                if (burningDuration <= 0)
                {
                    isBurning = false;
                }
            }

            if (isSpiked)
            {
                spikedDuration -= Time.deltaTime;
                if (spikedDuration <= 0)
                {
                    isSpiked = false;
                }
            }
        }

        public void OnMove(InputValue value)
        {
            _moveDirection = value.Get<Vector2>();
        }

        public void PickUpGun(Gun gun, GameObject collidedObject)
        {
            var o = gun.gameObject;
            if (collidedObject == gameObject)
                o.transform.parent = gunHolder;
            else
                o.transform.parent = collidedObject.transform;

            _guns.Add(gun);
            moveSpeed += gunMoveSpeedBonus;
            if(GameManger.Instance.difficulty != ModeSelect.Difficulty.Hard)
            {
                maxHealth += gun.gunHealthMax;
                _currentHealth += gun.gunHealthMax;
                _sb.Clear();
                healthText.text = _sb.Append(_currentHealth.ToString("F0")).Append("/").Append(maxHealth).ToString();
                healthSlider.maxValue = maxHealth;
                healthSlider.value = _currentHealth;
            }
            
            _totalDps += gun._gunDps;
            dpsText.text = "Total DPS :\n" + _totalDps.ToString("F0");

            // check the fartherst gun
            if (_farthestGun == null)
                _farthestGun = gun;
            else
            {
                var distance = Vector2.Distance(transform.position, gun.transform.position);
                if (distance > _farthestGunDistance)
                {
                    _farthestGun = gun;
                    _farthestGunDistance = distance;
                    _targetCameraSize = 10.45f + distance * 0.5f;
                }
            }
        }

        public void GetDamage(float damage)
        {
            _currentHealth -= damage;
            if(_currentHealth <= 0) 
                _currentHealth = 0;
            healthSlider.value = _currentHealth;
            _sb.Clear();
            healthText.text = _sb.Append(_currentHealth.ToString("F0")).Append("/").Append(maxHealth).ToString();
            
            StatisticManager.Instance.statisticData.totalGetDamage += damage;
            var originAvg = StatisticManager.Instance.statisticData.averageHealthRate;
            var hitCount = StatisticManager.Instance.statisticData.totalHitCount;
            StatisticManager.Instance.statisticData.averageHealthRate = (originAvg * hitCount + _currentHealth / maxHealth) / (hitCount + 1);
            StatisticManager.Instance.statisticData.totalHitCount += 1;
            
            // if(_currentHealth <= 0) 
            //     GameManger.Instance.LoseGame();
        }

        public void GetPercentageDamage(float percentage)
        {
            GetDamage(maxHealth * percentage);
        }

        public void DamageTextDead()
        {
            // do nothing
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void CleanupDestroyGuns()
        {
            _farthestGun = null;
            _farthestGunDistance = 0;
            _totalDps = 0;
            for (int i = _guns.Count - 1; i >= 0; i--)
            {
                if (_guns[i] == null)
                {
                    _guns.RemoveAt(i);
                }
                else
                {
                    _totalDps += _guns[i]._gunDps;
                    var distance = Vector2.Distance(transform.position, _guns[i].transform.position);
                    if (distance > _farthestGunDistance)
                    {
                        _farthestGun = _guns[i];
                        _farthestGunDistance = distance;
                    }
                }
            }
            dpsText.text = "Total DPS :\n" + _totalDps.ToString("F0");
            originMoveSpeed = 5f + _guns.Count * gunMoveSpeedBonus;
        }
        
        public void StartBurning(float damage, float duration, float burnSpeedRate)
        {
            buringDamage = damage;
            burningDuration = duration;
            this.burnSpeedRate = burnSpeedRate;
            isBurning = true;
        }
        
        public void StartSpiked(float duration, float speedRate)
        {
            spikedDuration = duration;
            spikeSpeedRate = speedRate;
            isSpiked = true;
        }

        public void PushBack(Vector3 directionNormalized, float speed, float time)
        {
            StartCoroutine(PushBackCoroutine(directionNormalized, speed, time));
        }
        
        private IEnumerator PushBackCoroutine(Vector3 directionNormalized, float speed, float time)
        {
            rb.velocity = directionNormalized * speed;
            canMove = false;
            yield return new WaitForSeconds(time);
            rb.velocity = Vector2.zero;
            canMove = true;
        }
        
        public float GetNowSpeed()
        {
            return moveSpeed;
        }
        
        public Vector2 GetNowDirection()
        {
            return _moveDirection * moveSpeed;
        }
        
        private IEnumerator TickDamage(float damage, float duration)
        {
            var tickCount = 0;
            while (tickCount < duration)
            {
                GetDamage(damage);
                yield return new WaitForSeconds(1f);
                tickCount++;
            }
        }
    }
}