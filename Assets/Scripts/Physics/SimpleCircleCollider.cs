using UnityEngine;

namespace Physics {
    public class SimpleCircleCollider : MonoBehaviour {
        [field: SerializeField, Min(float.Epsilon)]
        public float radius = 0.5f;

        private void OnEnable() => SimplePhysics2D.RegisterCollider(this);
        private void OnDisable() => SimplePhysics2D.DeregisterCollider(this);

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}