using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts
{
    public class GunDropRateManager : MonoBehaviour
    {
        // Singleton
        [SerializeField] private float failStackRate = 0.5f;
        public static GunDropRateManager Instance;
        private float _stackedAddRate = 0f;
        
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
        
        public bool TryDrop(float originTryRate)
        {
            if (originTryRate == 1f)
            {
                return true;
            }
            if (originTryRate + _stackedAddRate >= 1f)
            {
                _stackedAddRate -= 1f - originTryRate;
                return true;
            }
            
            if (Random.Range(0f, 1f) <= originTryRate + _stackedAddRate)
            {
                _stackedAddRate= 0f;
                return true;
            }
            else
            {
                _stackedAddRate += originTryRate * failStackRate;
                return false;
            }
        }
    }
}