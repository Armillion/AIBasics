using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Utility;

namespace Physics {
    public static class SimplePhysics2D {
        private static readonly List<Polygon> _closedGeometry = new ();
        private static readonly List<Polygon> _walls = new ();
        private static readonly List<SimpleCircleCollider> _colliders = new ();
        private static readonly List<SimpleCircleCollider> _collidersToRemove = new ();

        public static void SimulatePhysicsStep() {
            EnsureWallsZeroOverlap();
            EnsureColliderZeroOverlap();
            RemoveDeregisteredColliders();
        }
        public static void RegisterGeometry(Polygon[] closedGeometry, Polygon[] walls) {
            if (closedGeometry != null)
                _closedGeometry.AddRange(closedGeometry);

            if (walls != null)
                _walls.AddRange(walls);
        }

        public static void DeregisterGeometry(Polygon[] closedGeometry, Polygon[] walls) {
            foreach (Polygon polygon in closedGeometry)
                _closedGeometry.Remove(polygon);

            foreach (Polygon polygon in walls)
                _walls.Remove(polygon);
        }

        public static void RegisterCollider(SimpleCircleCollider collider) {
            _colliders.Add(collider);
            _collidersToRemove.Remove(collider);
        }

        public static void DeregisterCollider(SimpleCircleCollider collider) => _collidersToRemove.Add(collider);
        
        private static void RemoveDeregisteredColliders() {
            foreach (SimpleCircleCollider collider in _collidersToRemove)
                _colliders.Remove(collider);

            _collidersToRemove.Clear();
        }

        public static bool Raycast(
            Vector2 origin,
            Vector2 direction,
            out SimpleRaycastHit2D hit,
            params SimpleCircleCollider[] ignoreColliders
        ) {
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

        public static SimpleRaycastHit2D[] RaycastAll(
            Vector2 origin,
            Vector2 direction,
            params SimpleCircleCollider[] ignoreColliders
        ) {
            List<SimpleRaycastHit2D> hits = new ();

            foreach (Polygon polygon in _closedGeometry)
                if (Geometry.LinesIntersect(origin, origin + direction, polygon, true, out Vector2 point))
                    hits.Add(new SimpleRaycastHit2D { point = point });

            foreach (Polygon polygon in _walls)
                if (Geometry.LinesIntersect(origin, origin + direction, polygon, false, out Vector2 point))
                    hits.Add(new SimpleRaycastHit2D { point = point });

            foreach (SimpleCircleCollider collider in _colliders) {
                if (ignoreColliders != null && ignoreColliders.Contains(collider))
                    continue;

                if (Geometry.RayIntersectsCircle(
                        origin, direction, collider.transform.position, collider.radius, out Vector2 intersection1,
                        out Vector2 intersection2
                    )) {
                    Vector2 point = Vector2.Distance(origin, intersection1) < Vector2.Distance(origin, intersection2)
                        ? intersection1
                        : intersection2;

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

        private static void EnsureWallsZeroOverlap() {
            // foreach (SimpleCircleCollider collider in _colliders) {
            //     foreach (Polygon walls in _walls) {
            //         var vertices = (Vector2[])walls;
            //         
            //         for (var i = 0; i < vertices.Length; i++) {
            //             Vector2 direction = vertices[i] - vertices[(i + 1) % vertices.Length];
            //             Vector2 normal = new Vector2(-direction.y, direction.x).normalized;
            //             Vector2 overlap = normal * (collider.radius - Vector2.Dot(normal, (Vector2)collider.transform.position - vertices[i]));
            //             
            //             if (overlap.magnitude > 0)
            //                 collider.transform.position += (Vector3)overlap;
            //         }
            //     }
            // }
        }

        private struct EnsureZeroOverlapJob : IJobFor {
            public NativeArray<Vector2> positions;

            [ReadOnly]
            public NativeArray<float> radii;

            [ReadOnly]
            public NativeArray<bool> isStatic;
            
            [ReadOnly]
            public NativeArray<bool> isTrigger;

            public NativeList<(int, int)> contacts;

            public void Execute(int index) {
                if (isStatic[index]) return;
                
                Vector2 currentPosition = positions[index];
                float currentRadius = radii[index];

                for (var j = 0; j < positions.Length; j++) {
                    if (j == index) continue;

                    Vector2 otherPosition = positions[j];
                    float otherRadius = radii[j];

                    Vector2 direction = otherPosition - currentPosition;
                    float distance = direction.magnitude;
                    float overlap = currentRadius + otherRadius - distance;

                    if (overlap <= 0) continue;
                    contacts.Add((index, j));
                    if (isTrigger[index] || isTrigger[j]) continue;

                    float pushFactor = isStatic[j] ? 1f : 0.5f;
                    currentPosition -= direction.normalized * overlap * pushFactor;
                }

                positions[index] = currentPosition;
            }
        }

        private static void EnsureColliderZeroOverlap() {
            int count = _colliders.Count;

            var positions = new NativeArray<Vector2>(count, Allocator.TempJob);
            var radii = new NativeArray<float>(count, Allocator.TempJob);
            var isStatic = new NativeArray<bool>(count, Allocator.TempJob);
            var isTrigger = new NativeArray<bool>(count, Allocator.TempJob);
            var contacts = new NativeList<(int, int)>(count, Allocator.TempJob);

            for (var i = 0; i < count; i++) {
                SimpleCircleCollider collider = _colliders[i];
                positions[i] = collider.transform.position;
                radii[i] = collider.radius;
                isStatic[i] = collider.gameObject.isStatic;
                isTrigger[i] = collider.isTrigger;
            }

            var overlapJob = new EnsureZeroOverlapJob {
                positions = positions,
                radii = radii,
                isStatic = isStatic,
                isTrigger = isTrigger,
                contacts = contacts
            };

            JobHandle handle = overlapJob.Schedule(count, default);
            handle.Complete();

            for (var i = 0; i < count; i++)
                if (!isStatic[i])
                    _colliders[i].transform.position = positions[i];

            foreach ((int, int) contactPair in contacts) {
                SimpleCircleCollider collider1 = _colliders[contactPair.Item1];
                SimpleCircleCollider collider2 = _colliders[contactPair.Item2];
                
                if (!collider1.enabled || !collider2.enabled)
                    continue;

                switch (collider1.isTrigger) {
                    case true when !collider2.isTrigger:
                        collider1.OnTrigger(collider2);
                        break;

                    case false when collider2.isTrigger:
                        collider2.OnTrigger(collider1);
                        break;
                }
            }

            positions.Dispose();
            radii.Dispose();
            isStatic.Dispose();
            isTrigger.Dispose();
            contacts.Dispose();
        }
    }
}