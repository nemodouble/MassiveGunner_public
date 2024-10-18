using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.nemodouble.massiveGunner.Scripts
{
    public class ModeSelect : MonoBehaviour
    {
        public enum Difficulty
        {
            Easy,
            Normal,
            Hard
        }
        public Difficulty difficulty;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void OnClickHard()
        {
            difficulty = Difficulty.Hard;
            SceneManager.LoadScene("SampleScene");
        }
        
        public void OnClickEasy()
        {
            difficulty = Difficulty.Easy;
            SceneManager.LoadScene("SampleScene");
        }
    }
}