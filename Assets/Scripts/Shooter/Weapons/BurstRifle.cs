using Physics;
using UnityEngine;

namespace Shooter.Weapons {
    public class BurstRifle : Weapon {
        [SerializeField, Min(0)]
        private int burstCount = 3;
        
        [SerializeField, Min(0)]
        private float burstRate = 0.1f;
        
        public override async void Shoot(Vector3 origin, Vector3 direction) {
            if (fireRateTimer.IsRunning) return;
            fireRateTimer.Start();

            for (var i = 0; i < burstCount; i++) {
                Vector3 spread = Random.insideUnitCircle * angleAccuracy;
                direction = Quaternion.Euler(spread) * direction;
                
                if (!SimplePhysics2D.Raycast(origin, direction, out SimpleRaycastHit2D hit, _owner.Collider)) return;
                Debug.DrawLine(origin, hit.point, Color.red, 0.1f);
                if (!hit.transform || !hit.transform.TryGetComponent(out Health health)) return;
                
                health.TakeDamage(damage, _owner);
                await Awaitable.WaitForSecondsAsync(burstRate);
            }
        }
    }
}