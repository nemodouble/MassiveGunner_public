using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace com.nemodouble.massiveGunner.Scripts
{
    [Serializable]
    public struct StatisticData
    {
        public float totalDamage;
        public int totalKills;
        public float totalGetDamage;
        public int totalShoots;
        public float averageHealthRate;
        public int totalHitCount;
    }
}