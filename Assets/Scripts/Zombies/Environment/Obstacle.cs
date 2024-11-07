using System;
using UnityEngine;

namespace Zombies.Environment {
    [SelectionBase]
    public class Obstacle : MonoBehaviour {
        [SerializeField, Min(0f)]
        public float radius = 3f;
        
        [SerializeField]
        private Transform _visual;

        private void OnValidate() {
            if (_visual)
                _visual.localScale = new Vector3(radius, radius, 1f);
        }
    }
}