using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zombies.Steering {
    public class WanderBehaviour : MonoBehaviour, ISteeringBehaviour {
        [SerializeField, Min(0f)]
        private float _wanderRadius = 2f;
        
        [SerializeField, Min(0f)]
        private float _wanderDistance = 2f;
        
        [SerializeField, Min(0f)]
        private float _wanderJitter = 0.2f;
        
        [SerializeField, Min(0f)]
        private float _maxSteerForce = 2f;
        
        private Vector2 _wanderTarget;

        public Vector2 CalculateSteering(IVehicle vehicle) {
            _wanderTarget += Random.insideUnitCircle * _wanderJitter;
            _wanderTarget = _wanderTarget.normalized * _wanderRadius;

            Vector2 targetPosition = (Vector2)transform.position + vehicle.Velocity.normalized * _wanderDistance + _wanderTarget;
            Vector2 desiredVelocity = (targetPosition - (Vector2)transform.position).normalized * vehicle.MaxSpeed;
            Vector2 steer = Vector2.ClampMagnitude(desiredVelocity - vehicle.Velocity, _maxSteerForce);

            return steer;
        }

        private void OnDrawGizmos() {
            if (!enabled) return;
            
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.white;
            Vector3 circleCenter = Vector3.right * _wanderDistance;
            Gizmos.DrawLine(Vector3.zero, circleCenter);
            Gizmos.DrawWireSphere(circleCenter, _wanderRadius);
            Gizmos.DrawSphere(circleCenter + (Vector3)_wanderTarget, 0.1f);
            Gizmos.DrawLine(circleCenter, circleCenter + (Vector3)_wanderTarget);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}