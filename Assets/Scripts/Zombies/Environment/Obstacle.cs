using System;
using System.Collections.Generic;
using KBCore.Refs;
using Physics;
using UnityEngine;

namespace Zombies.Environment {
    [SelectionBase]
    [RequireComponent(typeof(SimpleCircleCollider))]
    public class Obstacle : MonoBehaviour {
        public static readonly HashSet<Obstacle> all = new();
        
        [SerializeField, Self]
        private SimpleCircleCollider _collider;
        
        [SerializeField, Min(0f)]
        public float radius = 3f;
        
        [SerializeField]
        private Transform _visual;

        private void OnValidate() {
            _collider.radius = radius;
            
            if (_visual)
                _visual.localScale = new Vector3(radius * 2f, radius * 2f, 1f);
        }

        private void OnEnable() => all.Add(this);

        private void OnDisable() => all.Remove(this);
    }
}