using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts
{
    public class StatisticManager : MonoBehaviour
    {
        // Singleton
        public static StatisticManager Instance;
        public StatisticData statisticData = new StatisticData();
        
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
        }
    }
}