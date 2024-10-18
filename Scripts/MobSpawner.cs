using System.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts
{
    public class MobSpawner : MonoBehaviour
    {
        public GameObject[] mobPrefabs;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private TextMeshProUGUI timeText;
        
        [Header("Spawn Time Settings")]
        [SerializeField] private float spawnInterval = 1f;
        [SerializeField] private float earlySpawnIntervalMultiplier = 30f;
        [SerializeField] private float spawnIntervalDownSpeed = 10f;
        
        [Space(10)]
        [SerializeField] private float spawnValueUpInterval = 60f;

        [Space(10)] 
        [ShowInInspector] private float _spawnLeftTime = 1f;
        [SerializeField] private float spendTime = 0f;

        [SerializeField] private GameObject BossFamilly;
        
        private StringBuilder _sb;
        
        
        private void Start()
        {
            mainCamera = Camera.main;
            _sb = new StringBuilder();
        }
        
        private void Update()
        {
            spendTime += Time.deltaTime;
            if (spendTime >= 300f)
            {
                BossFamilly.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
            
            if (_spawnLeftTime > 0)
            {
                _spawnLeftTime -= Time.deltaTime;
                if (_spawnLeftTime <= 0)
                {
                    SpawnMob();
                }
            }
            
            // Lv01 / 00:00
            int level = (int)(spendTime / spawnValueUpInterval + 1);
            int minutes = (int)(spendTime / 60);
            int seconds = (int)(spendTime % 60);
            
            _sb.Clear();
            _sb.Append("Lv").Append(level.ToString("D2"))
                .Append(" / ").Append(minutes.ToString("D2"))
                .Append(":").Append(seconds.ToString("D2"));
            
            timeText.text = _sb.ToString();
        }

        private void SpawnMob()
        {
            var spawnPos = GetRandomScreenEdge();
            var mobIndex = (int)Random.Range(0, spendTime / spawnValueUpInterval + 1);
            if (mobIndex >= mobPrefabs.Length)
            {
                mobIndex = mobPrefabs.Length - 1;
            }
            var go = Instantiate(mobPrefabs[mobIndex], (Vector2)mainCamera.ScreenToWorldPoint(spawnPos), Quaternion.identity);
            
            if(GameManger.Instance.difficulty == ModeSelect.Difficulty.Easy)
            {
                go.GetComponent<Enemy>().maxHealth *= 0.7f;
                go.GetComponent<Enemy>()._currentHealth *= 0.7f;
            }

            var enemy = go.GetComponent<Enemy>();
            enemy.maxHealth = 50f + spendTime * 2f;
            enemy._currentHealth = enemy.maxHealth;
            
            _spawnLeftTime = earlySpawnIntervalMultiplier / (spendTime + spawnIntervalDownSpeed) + spawnInterval;
        }
        
        private Vector2 GetRandomScreenEdge()
        {
            var selectedEdge = Random.Range(0, 4);
            return selectedEdge switch
            {
                0 => new Vector2(Random.Range(0, Screen.width), Screen.height),
                1 => new Vector2(Random.Range(0, Screen.width), 0),
                2 => new Vector2(Screen.width, Random.Range(0, Screen.height)),
                3 => new Vector2(0, Random.Range(0, Screen.height)),
                _ => throw new System.Exception("Invalid edge selected")
            };
        }
    }
}