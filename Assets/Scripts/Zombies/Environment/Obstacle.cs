using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zombies.Environment {
    [SelectionBase]
    public class Obstacle : MonoBehaviour {
        public static readonly HashSet<Obstacle> all = new();
        
        [SerializeField, Min(0f)]
        public float radius = 3f;
        
        [SerializeField]
        private Transform _visual;

        private void OnValidate() {
            if (_visual)
                _visual.localScale = new Vector3(radius, radius, 1f);
        }

        private void OnEnable() => all.Add(this);

        private void OnDisable() => all.Remove(this);
    }
}