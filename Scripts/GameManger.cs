using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.nemodouble.massiveGunner.Scripts
{
    public class GameManger : MonoBehaviour
    {
        // Singleton
        public static GameManger Instance;
        public Player player;
        public Camera mainCamera;

        public ModeSelect.Difficulty difficulty;

        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
        
        public ObjectPool bulletPool;
        public ObjectPool damageTextPool;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            var modeSelect = FindObjectOfType<ModeSelect>();
            if(modeSelect != null) difficulty = modeSelect.difficulty;
        }

        public void WinGame()
        {
            Time.timeScale = 0;
            winPanel.SetActive(true);
        }

        public void LoseGame()
        {
            Time.timeScale = 0;
            losePanel.SetActive(true);
        }
        
        public void Restart()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("MainScene");
        }
        
        public string ToKmbNumber(float number)
        {
            return number switch
            {
                < 1000 => number.ToString("F0"),
                < 1000000 => (number / 1000).ToString("F0") + "K",
                < 1000000000 => (number / 1000000).ToString("F0") + "M",
                _ => (number / 1000000000).ToString("F0") + "B"
            };
        }

        public string ToKmbNumber(int number)
        {
            return number switch
            {
                < 1000 => number.ToString(),
                < 1000000 => (number / 1000).ToString("F0") + "K",
                < 1000000000 => (number / 1000000).ToString("F0") + "M",
                _ => (number / 1000000000).ToString("F0") + "B"
            };
        }
    }
}