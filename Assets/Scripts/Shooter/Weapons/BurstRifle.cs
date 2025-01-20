using UnityEngine;

namespace Shooter.Weapons {
    public class BurstRifle : Weapon {
        [SerializeField, Min(0)]
        private int burstCount = 3;
        
        [SerializeField, Min(0)]
        private float burstRate = 0.1f;
        
        public override async void Shoot(Vector3 origin, Vector3 direction) {
            if (!CanFire) return;
            fireRateTimer.Start();

            for (var i = 0; i < burstCount; i++) {
                FireSingleBulletWithSpread(origin, direction);
                await Awaitable.WaitForSecondsAsync(burstRate);
            }
        }
    }
}