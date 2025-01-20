﻿using Physics;
using UnityEngine;

namespace Shooter.Weapons {
    public class Pistol : Weapon {
        public override void Shoot(Vector3 origin, Vector3 direction) {
            if (CurrentAmmo <= 0 || fireRateTimer.IsRunning) return;
            fireRateTimer.Start();
            
            CurrentAmmo--;
            Vector3 spread = Random.insideUnitCircle * _config.AngleAccuracy;
            direction = Quaternion.Euler(spread) * direction;
            
            Debug.DrawLine(origin, direction, Color.red, 0.1f);
            if (!SimplePhysics2D.Raycast(origin, direction, out SimpleRaycastHit2D hit, _owner.Collider)) return;
            if (!hit.transform || !hit.transform.TryGetComponent(out Health health)) return;
            
            health.TakeDamage(_config.Damage, _owner);
        }
    }
}
