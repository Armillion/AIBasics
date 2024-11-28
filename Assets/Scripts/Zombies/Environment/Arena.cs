using System;
using System.Collections.Generic;
using System.Linq;
using KBCore.Refs;
using Physics;
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
        public Vector2 Center => _bounds.center;
        public Vector2 Size => _bounds.size;

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
        
        private void Start() {
            SimplePhysics2D.RegisterGeometry(null, new [] { Walls });
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

        private void OnDrawGizmos() {
            Gizmos.color = Color.white;

            for (var i = 1; i < ((Vector2[])Walls).Length; i++) {
                Vector2 wallStart = ((Vector2[])Walls)[i - 1];
                Vector2 wallEnd = ((Vector2[])Walls)[i];
                
                Gizmos.DrawLine(wallStart, wallEnd);
            }
            
            Gizmos.DrawLine(((Vector2[])Walls).Last(), ((Vector2[])Walls).First());
        }
    }
}
