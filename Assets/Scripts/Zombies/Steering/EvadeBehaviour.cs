using System;
using KBCore.Refs;
using UnityEngine;

namespace Zombies.Steering {
    [RequireComponent(typeof(FleeBehaviour))]
    public class EvadeBehaviour : MonoBehaviour, ISteeringBehaviour {
        public IVehicle pursuer;

        [SerializeField, Self]
        private FleeBehaviour _fleeBehaviour;
        
        private void OnValidate() => this.ValidateRefs();

        public Vector2 CalculateSteering(IVehicle vehicle) {
            if (pursuer == null || !_fleeBehaviour) return Vector2.zero;
            
            Vector2 toPursuer = pursuer.Position - (Vector2)transform.position;
            float lookAheadTime = toPursuer.magnitude / (vehicle.MaxSpeed + pursuer.Velocity.magnitude);
            
            _fleeBehaviour.target = pursuer.Position + pursuer.Velocity * lookAheadTime;
            return _fleeBehaviour.CalculateSteering(vehicle);
        }
    }
}