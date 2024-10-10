using System;
using UnityEngine;

namespace Weapons {
    public class Bullet : MonoBehaviour {
        [SerializeField]
        private float _speed = 10f;
        
        [SerializeField, Min(0f)]
        private float _lifetime = 5f;

        private void Update() {
            transform.position += transform.right * (_speed * Time.deltaTime);
        }
        
        public void Fire() {
            gameObject.SetActive(true);
            Destroy(gameObject, _lifetime);
        }
    }
}