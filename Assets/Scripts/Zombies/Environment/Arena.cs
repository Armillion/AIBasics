using System;
using System.Collections.Generic;
using System.Linq;
using KBCore.Refs;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Zombies.Environment {
    public class Arena : MonoBehaviour {
        [field: SerializeField]
        public Polygon Walls { get; private set; }
        
        [SerializeField, Child]
        private Obstacle[] _obstacles = Array.Empty<Obstacle>();
        
        public IReadOnlyCollection<Obstacle> Obstacles => _obstacles;

        private Bounds _bounds;
        
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

        private void Awake() {
            _bounds = new Bounds(Walls.vertices[0], Vector3.zero);
            
            foreach (Vector2 vertex in Walls.vertices)
                _bounds.Encapsulate(vertex);
        }

        private Vector2 RandomPoint() => new(
            Random.Range(_bounds.min.x, _bounds.max.x),
            Random.Range(_bounds.min.y, _bounds.max.y)
        );

        public Vector2 RandomSpawnablePoint() {
            const int maxAttempts = 100;

            for (var i = 0; i < maxAttempts; i++) {
                Vector2 point = RandomPoint();

                if (Geometry.IsPointInPolygon(point, Walls) && !IsPointInObstacle(point))
                    return point;
            }
            
            return Vector2.zero;
        }
        
        private bool IsPointInObstacle(Vector2 point)
            => _obstacles.Any(obstacle => Geometry.IsInsideCircle(point, obstacle.transform.position, obstacle.radius));
    }
}
