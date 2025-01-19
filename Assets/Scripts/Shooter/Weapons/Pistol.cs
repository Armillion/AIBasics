using Physics;
using UnityEngine;

namespace Shooter.Weapons {
    public class Pistol : Weapon {
        public override void Shoot(Vector3 origin, Vector3 direction, params SimpleCircleCollider[] ignoreColliders) {
            if (FireRateTimer.IsRunning) return;
            
            Vector3 spread = Random.insideUnitCircle * angleAccuracy;
            direction = Quaternion.Euler(spread) * direction;
            
            if (!SimplePhysics2D.Raycast(origin, direction, out SimpleRaycastHit2D hit, ignoreColliders)) return;
            if (!hit.transform || !hit.transform.TryGetComponent(out Health health)) return;
            
            health.TakeDamage(damage);
            FireRateTimer.Start();
        }
    }
}
