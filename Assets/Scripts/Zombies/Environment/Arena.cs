using System;
using System.Collections.Generic;
using KBCore.Refs;
using Shooter.Environment;
using UnityEngine;

namespace Zombies.Environment {
    public class Arena : MonoBehaviour {
        [field: SerializeField]
        public Polygon Walls { get; private set; }
        
        [SerializeField, Child]
        private Obstacle[] _obstacles = Array.Empty<Obstacle>();
        
        public IReadOnlyCollection<Obstacle> Obstacles => _obstacles;

#if UNITY_EDITOR
        private void Reset() {
            Walls = new Polygon {
                vertices = new Vector2[] {
                    new(-5f, -5f),
                    new(5f, -5f),
                    new(5f, 5f),
                    new(-5f, 5f)
                }
            };
        }
#endif

        private void OnValidate() => this.ValidateRefs();
    }
}
