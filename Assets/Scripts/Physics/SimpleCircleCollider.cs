using System;
using UnityEngine;

namespace Physics {
    public class SimpleCircleCollider : MonoBehaviour {
        public bool isTrigger;
        
        [field: SerializeField, Min(float.Epsilon)]
        public float radius = 0.5f;

        private void OnEnable() => SimplePhysics2D.RegisterCollider(this);
        private void OnDisable() => SimplePhysics2D.DeregisterCollider(this);

        public void OnTrigger(SimpleCircleCollider other)
            => gameObject.SendMessage("OnSimpleTrigger", other, SendMessageOptions.DontRequireReceiver);

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}