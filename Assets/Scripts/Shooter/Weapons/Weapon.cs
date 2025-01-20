using ImprovedTimers;
using KBCore.Refs;
using Physics;
using Shooter.Agents;
using TMPro;
using UnityEngine;
using Utility.DescriptiveGizmos;

namespace Shooter.Weapons {
    public abstract class Weapon : MonoBehaviour {
        [SerializeField, Parent]
        protected Agent _owner;
        
        [field: SerializeField]
        protected WeaponConfig _config;
        
        [SerializeField]
        private int _currentAmmo;
        
        [SerializeField]
        private TMP_Text _ammoText;

        public int CurrentAmmo {
            get => _currentAmmo;
            set => _currentAmmo = Mathf.Min(_config.MaxAmmo, value);
        }

        public int MaxAmmo => _config.MaxAmmo;
        public int Damage => _config.Damage;
        
        protected bool CanFire => CurrentAmmo > 0 && !fireRateTimer.IsRunning;
        
        protected CountdownTimer fireRateTimer;

        private void OnValidate() => this.ValidateRefs();

        private void Start() {
            CurrentAmmo = _config.MaxAmmo;
            fireRateTimer = new CountdownTimer(_config.FireRate);
        }

        public abstract void Shoot(Vector3 origin, Vector3 direction);

        protected void FireSingleBulletWithSpread(Vector3 origin, Vector3 direction) {
            CurrentAmmo--;
            _ammoText.text = CurrentAmmo.ToString();
            
            Vector3 spread = Random.insideUnitCircle * _config.AngleAccuracy;
            direction = Quaternion.Euler(spread) * direction;
                
            if (!SimplePhysics2D.Raycast(origin, direction, out SimpleRaycastHit2D hit, _owner.Collider)) return;
            Debug.DrawLine(origin, hit.point, Color.red, 0.1f);
            if (!hit.transform || !hit.transform.TryGetComponent(out Health health)) return;
                
            health.TakeDamage(_config.Damage, _owner);
        }
        
        private void OnDrawGizmosSelected() {
            if (!_config) return;
            
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            const float lineLength = 5f;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, _config.AngleAccuracy) * Vector2.up * lineLength);
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, -_config.AngleAccuracy) * Vector2.up * lineLength);
            Gizmos.matrix = Matrix4x4.identity;

#if UNITY_EDITOR
            GizmosLegend.AddLabel(this, "Accuracy", Color.red, GizmoType.Line);
#endif
        }

#if UNITY_EDITOR
        private void OnDestroy() => GizmosLegend.Unregister(this);
#endif
    }
}