using System;
using TMPro;
using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts
{
    public class Tutorial : MonoBehaviour
    {
        private void Awake()
        {
            // load the state
            if (PlayerPrefs.GetInt("Tutorial", 0) == 1)
            {
                Destroy(gameObject);
            }
        }

        public void GoAway()
        {
            // save the state
            PlayerPrefs.SetInt("Tutorial", 1);
            Destroy(gameObject);
        }
    }
}