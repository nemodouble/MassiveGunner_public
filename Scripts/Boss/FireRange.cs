using System.Collections;
using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts.Boss
{
    public class FireRange : Trap
    {
        protected override void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.TryGetComponent(typeof(IBurnable), out var obj2))
            {
                var player = (IBurnable)obj2;
                player.StartBurning(percentDamage, slowDuration, speedChangeRate);
            }
        }
    }
}