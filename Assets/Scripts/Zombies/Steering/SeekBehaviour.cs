using System;
using DefaultNamespace;
using UnityEngine;

namespace Zombies.Steering {
    public class SeekBehaviour : MonoBehaviour, ISteeringBehaviour {
        [SerializeField]
        private Transform _target;
        
        [SerializeField, Min(0f)]
        private float _steeringForce = 2f;
        
        public Vector2 CalculateSteering(IVehicle vehicle) {
            Vector2 desired = _target.position - transform.position;
            desired = Vector2.ClampMagnitude(desired, vehicle.MaxSpeed);
            Vector2 steering = desired - vehicle.Velocity;
            steering = Vector2.ClampMagnitude(steering, _steeringForce);
            return steering;
        }
    }
}