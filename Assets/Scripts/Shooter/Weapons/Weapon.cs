using ImprovedTimers;
using Physics;
using UnityEngine;
using Utility.DescriptiveGizmos;

namespace Shooter.Weapons {
    public abstract class Weapon : MonoBehaviour {
        [field: SerializeField, Min(0f)]
        private float fireRate = 1f;
        
        [field: SerializeField, Min(0)]
        protected int damage = 15;
        
        [field: SerializeField, Min(0f)]
        protected float angleAccuracy = 2f;

        protected CountdownTimer FireRateTimer;

        private void Start() => FireRateTimer = new CountdownTimer(fireRate);

        public abstract void Shoot(Vector3 origin, Vector3 direction, params SimpleCircleCollider[] ignoreColliders);

        private void OnDrawGizmosSelected() {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            const float lineLength = 5f;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, angleAccuracy) * Vector2.up * lineLength);
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, -angleAccuracy) * Vector2.up * lineLength);
            Gizmos.matrix = Matrix4x4.identity;

#if UNITY_EDITOR
            GizmosLegend.AddLabel(this, "Accuracy", Color.red, GizmoType.Line);
#endif
        }
    }
}