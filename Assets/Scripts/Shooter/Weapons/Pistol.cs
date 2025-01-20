using UnityEngine;

namespace Shooter.Weapons {
    public class Pistol : Weapon {
        public override void Shoot(Vector3 origin, Vector3 direction) {
            if (!CanFire) return;
            fireRateTimer.Start();
            FireSingleBulletWithSpread(origin, direction);
        }
    }
}
