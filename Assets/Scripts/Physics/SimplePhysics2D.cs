using System.Collections.Generic;
using System.Linq;
using Shooter.Agents;
using Shooter.Environment;
using UnityEngine;
using Utility;

namespace Physics {
    public static class SimplePhysics2D {
        private static readonly List<Polygon> _closedGeometry = new();
        private static readonly List<Polygon> _walls = new();
        private static readonly List<SimpleCircleCollider> _colliders = new();

        public static void EnsureZeroOverlap() {
            foreach (SimpleCircleCollider collider1 in _colliders) {
                foreach (SimpleCircleCollider collider2 in _colliders) {
                    if (collider1 == collider2) continue;
                    
                    Vector2 direction = collider2.transform.position - collider1.transform.position;
                    float overlap = collider1.radius + collider2.radius - direction.magnitude;
                    
                    if (overlap >= 0) 
                        collider1.transform.position -= (Vector3)direction.normalized * overlap * 0.5f;
                }
            }
        }
        
        public static void RegisterGeometry(Polygon[] closedGeometry, Polygon[] walls) {
            _closedGeometry.AddRange(closedGeometry);
            _walls.AddRange(walls);
        }
        
        public static void DeregisterGeometry(Polygon[] closedGeometry, Polygon[] walls) {
            foreach (Polygon polygon in closedGeometry)
                _closedGeometry.Remove(polygon);
            
            foreach (Polygon polygon in walls)
                _walls.Remove(polygon);
        }

        public static void RegisterCollider(SimpleCircleCollider collider) => _colliders.Add(collider);
        public static void DeregisterCollider(SimpleCircleCollider collider) => _colliders.Remove(collider);
        
        public static bool Raycast(Vector2 origin, Vector2 direction, out SimpleRaycastHit2D hit, params SimpleCircleCollider[] ignoreColliders) {
            SimpleRaycastHit2D[] hits = RaycastAll(origin, direction, ignoreColliders);
            
            if (hits.Length == 0) {
                hit = new SimpleRaycastHit2D();
                return false;
            }
            
            var minDistance = float.MaxValue;
            hit = new SimpleRaycastHit2D();
            
            foreach (SimpleRaycastHit2D raycastHit2D in hits) {
                float distance = Vector2.Distance(origin, raycastHit2D.point);
                if (distance >= minDistance) continue;
                minDistance = distance;
                hit = raycastHit2D;
            }
            
            return true;
        }

        public static SimpleRaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, params SimpleCircleCollider[] ignoreColliders) {
            List<SimpleRaycastHit2D> hits = new();
            
            foreach (Polygon polygon in _closedGeometry)
                if (Geometry.LinesIntersect(origin, origin + direction, polygon, true, out Vector2 point))
                    hits.Add(new SimpleRaycastHit2D { point = point });
            
            foreach (Polygon polygon in _walls)
                if (Geometry.LinesIntersect(origin, origin + direction, polygon, false, out Vector2 point))
                    hits.Add(new SimpleRaycastHit2D { point = point });
            
            foreach (SimpleCircleCollider collider in _colliders) {
                if (ignoreColliders != null && ignoreColliders.Contains(collider))
                    continue;
                
                if (Geometry.RayIntersectsCircle(origin, direction, collider.transform.position, collider.radius, out Vector2 intersection1, out Vector2 intersection2)) {
                    Vector2 point = Vector2.Distance(origin, intersection1) < Vector2.Distance(origin, intersection2) ? intersection1 : intersection2;
                    hits.Add(new SimpleRaycastHit2D { point = point, transform = collider.transform });
                }
            }
            
            return hits.ToArray();
        }
        
        public static SimpleCircleCollider OverlapCircle(Vector2 position, float radius) {
            foreach (SimpleCircleCollider collider in _colliders)
                if (Geometry.IsInsideCircle(collider.transform.position, position, radius + collider.radius))
                    return collider;

            return null;
        }

        public static void Clear() {
            _closedGeometry.Clear();
            _walls.Clear();
            _colliders.Clear();
        }
    }
}