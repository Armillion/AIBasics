using UnityEngine;

namespace Physics {
    public class SimpleCircleCollider : MonoBehaviour {
        [field: SerializeField, Min(float.Epsilon)]
        public float Radius { get; private set; } = 0.5f;

        private void Awake() => SimplePhysics2D.RegisterCollider(this);
        private void OnDestroy() => SimplePhysics2D.DeregisterCollider(this);

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}