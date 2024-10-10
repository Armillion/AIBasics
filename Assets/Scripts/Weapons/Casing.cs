using System;
using System.Collections;
using UnityEngine;

namespace Weapons {
    public class Casing : MonoBehaviour {
        private const int MAX_CASING = 256;
        private static readonly Queue _casings = new(MAX_CASING);
        
        [SerializeField]
        private float _drag = 0.95f;
        
        private Vector2 _velocity;

        public void Eject(Vector2 direction, float force) {
            if (_casings.Count > MAX_CASING) {
                var oldestCasing = (GameObject)_casings.Dequeue();
                Destroy(oldestCasing);
            }
            
            _casings.Enqueue(gameObject);
            _velocity = direction.normalized * force;
        }

        private void Update() {
            transform.position += (Vector3)(_velocity * Time.deltaTime);
            _velocity *= _drag;

            if (_velocity.magnitude < 0.01f)
                Destroy(this);
        }
    }
}