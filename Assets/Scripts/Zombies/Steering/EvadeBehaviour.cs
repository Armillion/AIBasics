using System;
using UnityEngine;

namespace Zombies.Steering {
    [RequireComponent(typeof(FleeBehaviour))]
    public class EvadeBehaviour : MonoBehaviour, ISteeringBehaviour {
        public IVehicle pursuer;

        private FleeBehaviour _fleeBehaviour;

        private void Start() {
            _fleeBehaviour = GetComponent<FleeBehaviour>();
        }

        public Vector2 CalculateSteering(IVehicle vehicle) {
            if (pursuer == null) return Vector2.zero;
            
            Vector2 toPursuer = pursuer.Position - (Vector2)transform.position;
            float lookAheadTime = toPursuer.magnitude / (vehicle.MaxSpeed + pursuer.Velocity.magnitude);
            
            _fleeBehaviour.target = pursuer.Position + pursuer.Velocity * lookAheadTime;
            return _fleeBehaviour.CalculateSteering(vehicle);
        }
    }
}