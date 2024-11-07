using System;
using UnityEngine;
using Zombies.Steering;

namespace Zombies {
    [SelectionBase]
    public class Zombie : MonoBehaviour, IVehicle {
        [Serializable]
        private struct SteeringBehaviourConfig {
            [SerializeField]
            private Component _component;
            public float weight;
            
            public ISteeringBehaviour Behaviour => _component as ISteeringBehaviour;

            public void OnValidate(Component validator = null) {
                if (!_component || Behaviour != null) return;
                Debug.LogError($"{_component} does not implement {nameof(ISteeringBehaviour)}", validator);
                _component = null;
            }
        }
        
        [field: SerializeField, Min(0f)]
        public float MaxSpeed { get; private set; } = 2f;

        [SerializeField]
        private SteeringBehaviourConfig[] _steeringBehaviours;
        
        public Vector2 Position => transform.position;
        public Vector2 Velocity { get; private set; }

        private void OnValidate() {
            foreach (SteeringBehaviourConfig steeringBehaviour in _steeringBehaviours)
                steeringBehaviour.OnValidate(this);
        }

        private void Update() {
            Vector2 steering = CalculateSteering();
            
            Velocity += steering * Time.deltaTime;
            // Velocity = Vector2.ClampMagnitude(Velocity, MaxSpeed);

            transform.position += (Vector3)Velocity * Time.deltaTime;

            if (Velocity.magnitude <= 0.1f) return;
            
            float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private Vector2 CalculateSteering() {
            Vector2 steering = Vector2.zero;

            foreach (SteeringBehaviourConfig steeringBehaviour in _steeringBehaviours) {
                Vector2 forceToAdd = steeringBehaviour.Behaviour.CalculateSteering(this) * steeringBehaviour.weight;
                if (!AccumulateForce(ref steering, forceToAdd)) break;
            }

            return steering;
        }

        private bool AccumulateForce(ref Vector2 accumulatedForce, Vector2 forceToAdd) {
            float magnitudeSoFar = accumulatedForce.magnitude;
            float magnitudeRemaining = MaxSpeed - magnitudeSoFar;
            
            if (magnitudeRemaining <= 0f) return false;
            
            float magnitudeToAdd = forceToAdd.magnitude;
            
            if (magnitudeToAdd < magnitudeRemaining)
                accumulatedForce += forceToAdd;
            else
                accumulatedForce += forceToAdd.normalized * magnitudeRemaining;
            
            return true;
        }
    }
}