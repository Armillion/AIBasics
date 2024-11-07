using System;
using KBCore.Refs;
using UnityEngine;

namespace Zombies.Steering {
    [RequireComponent(typeof(SeekBehaviour))]
    public class PursuitBehaviour : MonoBehaviour, ISteeringBehaviour {
        public IVehicle Target { get; set; }
                
        [SerializeField, Self]
        private SeekBehaviour _seekBehaviour;

        private void OnValidate() => this.ValidateRefs();

        public Vector2 CalculateSteering(IVehicle vehicle) {
            Vector2 toTarget = Target.Position - vehicle.Position;
            float relativeHeading = Vector2.Dot(vehicle.Velocity.normalized, Target.Velocity.normalized);
            
            if (Vector2.Dot(toTarget, vehicle.Velocity) > 0 && relativeHeading < -0.95f)
                return _seekBehaviour.CalculateSteering(vehicle);
            
            float lookAheadTime = toTarget.magnitude / (vehicle.MaxSpeed + Target.Velocity.magnitude);
            Vector2 predictedPosition = Target.Position + Target.Velocity * lookAheadTime;
            
            _seekBehaviour.target = predictedPosition;
            return _seekBehaviour.CalculateSteering(vehicle);
        }
    }
}